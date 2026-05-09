using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;

namespace GSP.Mes
{
    public class FtpFileManager
    {
        #region 属性配置
        public string FtpServer { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 21;
        public bool UsePassive { get; set; } = true;
        public bool EnableSsl { get; set; } = false;
        public int Timeout { get; set; } = 30000; // 默认30秒超时
        #endregion
        #region 事件定义
        public event EventHandler<FtpProgressEventArgs> ProgressChanged;
        public event EventHandler<FtpErrorEventArgs> ErrorOccurred;
        public event EventHandler<string> UploadCompleted;
        public event EventHandler<string> DownloadCompleted;
        public event EventHandler<string> FileDeleted;
        public event EventHandler<string> DirectoryCreated;
        #endregion
        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            // 示例：释放可能存在的资源（根据实际需求扩展）
            // 若当前无资源需要释放，可留空或移除IDisposable接口
        }

        #region 核心上传方法（同步+异步）
        public void UploadFile(string localFilePath, string remoteFilePath,
                           bool overwrite = true, bool createDirectory = true)
        {
            ValidateInputs(localFilePath, remoteFilePath);

            try
            {
                // 确保远程目录存在
                if (createDirectory)
                {
                    string remoteDir = GetDirectoryFromPath(remoteFilePath);
                    CreateDirectoryRecursive(remoteDir);
                }

                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.UploadFile);
                using (var fileStream = File.OpenRead(localFilePath))
                {
                    client.ContentLength = fileStream.Length;

                    using (var requestStream = client.GetRequestStream())
                    {
                        TransferData(fileStream, requestStream, fileStream.Length, isUpload: true);
                    }
                }

                // 验证上传结果
                VerifyFileExists(remoteFilePath, fileSize: new FileInfo(localFilePath).Length);

                UploadCompleted?.Invoke(this, $"成功上传: {Path.GetFileName(localFilePath)}");
            }
            catch (Exception ex)
            {
                HandleError(ex, localFilePath);
            }
        }

        public async Task UploadFileAsync(string localFilePath, string remoteFilePath,
                                    bool overwrite = true, bool createDirectory = true,
                                    CancellationToken cancellationToken = default)
        {
            ValidateInputs(localFilePath, remoteFilePath);

            try
            {
                // 确保远程目录存在
                if (createDirectory)
                {
                    string remoteDir = GetDirectoryFromPath(remoteFilePath);
                    await CreateDirectoryRecursiveAsync(remoteDir, cancellationToken);
                }

                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.UploadFile);
                using (var fileStream = File.OpenRead(localFilePath))
                {
                    client.ContentLength = fileStream.Length;

                    using (var requestStream = await client.GetRequestStreamAsync().ConfigureAwait(false))
                    {
                        await TransferDataAsync(fileStream, requestStream, fileStream.Length,
                                              isUpload: true, cancellationToken);
                    }
                }

                // 验证上传结果
                await VerifyFileExistsAsync(remoteFilePath, new FileInfo(localFilePath).Length, cancellationToken);

                UploadCompleted?.Invoke(this, $"成功上传: {Path.GetFileName(localFilePath)}");
            }
            catch (Exception ex)
            {
                HandleError(ex, localFilePath);
            }
        }
        #endregion

        #region 文件下载方法（同步+异步）
        public void DownloadFile(string remoteFilePath, string localFilePath)
        {
            ValidateDownloadInputs(remoteFilePath, localFilePath);
            try
            {
                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.DownloadFile);
                using (var response = (FtpWebResponse)client.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var fileStream = File.Create(localFilePath))
                {
                  TransferData(responseStream, fileStream, response.ContentLength, isUpload: false);
                }

                DownloadCompleted?.Invoke(this, $"成功下载: {Path.GetFileName(localFilePath)}");
            }
            catch (Exception ex)
            {
                HandleError(ex, remoteFilePath);
            }
        }

        public async Task DownloadFileAsync(string remoteFilePath, string localFilePath,
                                          CancellationToken cancellationToken = default)
        {
            ValidateDownloadInputs(remoteFilePath, localFilePath);

            try
            {
                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.DownloadFile);
                using (var response = (FtpWebResponse)await client.GetResponseAsync().ConfigureAwait(false))
                using (var responseStream = response.GetResponseStream())
                using (var fileStream = File.Create(localFilePath))
                {
                    await TransferDataAsync(responseStream, fileStream, response.ContentLength,
                                         isUpload: false, cancellationToken);
                }

                DownloadCompleted?.Invoke(this, $"成功下载: {Path.GetFileName(localFilePath)}");
            }
            catch (Exception ex)
            {
                HandleError(ex, remoteFilePath);
            }
        }
        #endregion

        #region 文件删除方法（同步+异步）
        public void DeleteFile(string remoteFilePath)
        {
            ValidateRemotePath(remoteFilePath);

            try
            {
                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.DeleteFile);
                using (var response = (FtpWebResponse)client.GetResponse())
                {
                    if (response.StatusCode == FtpStatusCode.FileActionOK)
                    {
                        FileDeleted?.Invoke(this, $"成功删除: {remoteFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, remoteFilePath);
            }
        }

        public async Task DeleteFileAsync(string remoteFilePath,
                                        CancellationToken cancellationToken = default)
        {
            ValidateRemotePath(remoteFilePath);

            try
            {
                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.DeleteFile);
                using (var response = (FtpWebResponse)await client.GetResponseAsync().ConfigureAwait(false))
                {
                    if (response.StatusCode == FtpStatusCode.FileActionOK)
                    {
                        FileDeleted?.Invoke(this, $"成功删除: {remoteFilePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, remoteFilePath);
            }
        }
        #endregion

        #region 目录操作方法（新增递归创建和存在检查）
        /// <summary>
        /// 递归创建目录（确保整个路径存在）
        /// </summary>
        public void CreateDirectoryRecursive(string remoteDirectoryPath)
        {
            ValidateRemotePath(remoteDirectoryPath);
            if (DirectoryExists(remoteDirectoryPath)) return;

            string[] parts = remoteDirectoryPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = "";

            foreach (string part in parts)
            {
                currentPath += "/" + part;
                if (!DirectoryExists(currentPath))
                {
                    CreateSingleDirectory(currentPath);
                }
            }
        }

        /// <summary>
        /// 异步递归创建目录
        /// </summary>
        public async Task CreateDirectoryRecursiveAsync(string remoteDirectoryPath,
                                                      CancellationToken cancellationToken = default)
        {
            ValidateRemotePath(remoteDirectoryPath);
            if (await DirectoryExistsAsync(remoteDirectoryPath, cancellationToken)) return;

            string[] parts = remoteDirectoryPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = "";

            foreach (string part in parts)
            {
                currentPath += "/" + part;
                if (!await DirectoryExistsAsync(currentPath, cancellationToken))
                {
                    await CreateSingleDirectoryAsync(currentPath, cancellationToken);
                }
            }
        }

        /// <summary>
        /// 检查目录是否存在
        /// </summary>
        public bool DirectoryExists(string remoteDirectoryPath)
        {
            ValidateRemotePath(remoteDirectoryPath);
            if (remoteDirectoryPath == "/") return true; // 根目录始终存在

            try
            {
                // 尝试列出目录内容以验证存在性
                var client = CreateFtpClient(remoteDirectoryPath, WebRequestMethods.Ftp.ListDirectory);
                using (var response = (FtpWebResponse)client.GetResponse())
                {
                    return response.StatusCode == FtpStatusCode.DataAlreadyOpen;
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 异步检查目录是否存在
        /// </summary>
        public async Task<bool> DirectoryExistsAsync(string remoteDirectoryPath,
                                                   CancellationToken cancellationToken = default)
        {
            ValidateRemotePath(remoteDirectoryPath);
            if (remoteDirectoryPath == "/") return true;

            try
            {
                var client = CreateFtpClient(remoteDirectoryPath, WebRequestMethods.Ftp.ListDirectory);
                using (var response = (FtpWebResponse)await client.GetResponseAsync().ConfigureAwait(false))
                {
                    return response.StatusCode == FtpStatusCode.DataAlreadyOpen;
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 创建单个目录（内部方法）
        /// </summary>
        private void CreateSingleDirectory(string remoteDirectoryPath)
        {
            try
            {
                var client = CreateFtpClient(remoteDirectoryPath, WebRequestMethods.Ftp.MakeDirectory);
                using (var response = (FtpWebResponse)client.GetResponse())
                {
                    if (response.StatusCode == FtpStatusCode.PathnameCreated)
                    {
                        DirectoryCreated?.Invoke(this, $"成功创建目录: {remoteDirectoryPath}");
                    }
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                // 目录已存在，忽略错误
            }
        }

        /// <summary>
        /// 异步创建单个目录（内部方法）
        /// </summary>
        private async Task CreateSingleDirectoryAsync(string remoteDirectoryPath,
                                                    CancellationToken cancellationToken = default)
        {
            try
            {
                var client = CreateFtpClient(remoteDirectoryPath, WebRequestMethods.Ftp.MakeDirectory);
                using (var response = (FtpWebResponse)await client.GetResponseAsync().ConfigureAwait(false))
                {
                    if (response.StatusCode == FtpStatusCode.PathnameCreated)
                    {
                        DirectoryCreated?.Invoke(this, $"成功创建目录: {remoteDirectoryPath}");
                    }
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                // 目录已存在，忽略错误
            }
        }
        #endregion

        #region 辅助方法
        /// 从文件路径中提取目录路径
        /// </summary>
        private string GetDirectoryFromPath(string filePath)
        {
            int lastSlash = filePath.LastIndexOf('/');
            if (lastSlash <= 0) return "/";
            return filePath.Substring(0, lastSlash);
        }
        /// <summary>
        /// 验证文件是否成功上传
        /// </summary>
        private void VerifyFileExists(string remoteFilePath, long fileSize)
        {
            try
            {
                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.GetFileSize);
                using (var response = (FtpWebResponse)client.GetResponse())
                {
                    if (response.ContentLength != fileSize)
                    {
                        throw new ApplicationException($"文件上传不完整: 预期大小 {fileSize} 字节, 实际大小 {response.ContentLength} 字节");
                    }
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                throw new ApplicationException("文件上传失败: 目标文件不存在");
            }
        }
        /// <summary>
        /// 异步验证文件是否成功上传
        /// </summary>
        private async Task VerifyFileExistsAsync(string remoteFilePath, long fileSize, CancellationToken cancellationToken)
        {
            try
            {
                var client = CreateFtpClient(remoteFilePath, WebRequestMethods.Ftp.GetFileSize);
                using (var response = (FtpWebResponse)await client.GetResponseAsync().ConfigureAwait(false))
                {
                    if (response.ContentLength != fileSize)
                    {
                        throw new ApplicationException($"文件上传不完整: 预期大小 {fileSize} 字节, 实际大小 {response.ContentLength} 字节");
                    }
                }
            }
            catch (WebException ex) when ((ex.Response as FtpWebResponse)?.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                throw new ApplicationException("文件上传失败: 目标文件不存在");
            }
        }
        /// <summary>
        /// 增强版的FTP客户端创建方法
        /// </summary>
        private FtpWebRequest CreateFtpClient(string remotePath, string method)
        {
            // 规范化路径
            remotePath = remotePath.TrimStart('/');
            var uri = new UriBuilder("ftp", FtpServer, Port, remotePath).Uri;

            var request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = new NetworkCredential(Username, Password);
            request.Method = method;
            request.UsePassive = UsePassive;
            request.EnableSsl = EnableSsl;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;

            return request;
        }

        private void TransferData(Stream source, Stream destination, long totalBytes, bool isUpload)
        {
            var buffer = new byte[4096];
            int bytesRead;
            long totalTransferred = 0;

            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, bytesRead);
                totalTransferred += bytesRead;
                ReportProgress(totalTransferred, totalBytes, isUpload);
            }
        }

        private async Task TransferDataAsync(Stream source, Stream destination, long totalBytes,
                                           bool isUpload, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];
            int bytesRead;
            long totalTransferred = 0;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)
                   .ConfigureAwait(false)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken)
                                .ConfigureAwait(false);
                totalTransferred += bytesRead;
                ReportProgress(totalTransferred, totalBytes, isUpload);
            }
        }

        private void ValidateInputs(string localPath, string remotePath)
        {
            if (!File.Exists(localPath))
                throw new FileNotFoundException("本地文件不存在", localPath);

            ValidateRemotePath(remotePath);
        }

        private void ValidateDownloadInputs(string remotePath, string localPath)
        {
            ValidateRemotePath(remotePath);

            if (string.IsNullOrWhiteSpace(localPath))
                throw new ArgumentException("本地路径不能为空");

            var directory = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
        }

        private void ValidateRemotePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("远程路径不能为空");

            if (string.IsNullOrWhiteSpace(FtpServer))
                throw new InvalidOperationException("FTP服务器未配置");
        }

        private void ReportProgress(long bytesTransferred, long totalBytes, bool isUpload)
        {
            if (totalBytes <= 0) return; // 防止除零错误

            var args = new FtpProgressEventArgs
            {
                BytesTransferred = bytesTransferred,
                TotalBytes = totalBytes,
                ProgressPercentage = (int)((bytesTransferred * 100) / totalBytes),
                IsUpload = isUpload
            };

            ProgressChanged?.Invoke(this, args);
        }

        private void HandleError(Exception ex, string filePath)
        {
            var errorArgs = new FtpErrorEventArgs
            {
                FileName = Path.GetFileName(filePath),
                ErrorMessage = $"操作失败: {ex.Message}",
                Exception = ex
            };

            ErrorOccurred?.Invoke(this, errorArgs);
        }
        #endregion
    }

    #region 事件参数类
    public class FtpProgressEventArgs : EventArgs
    {
        public long BytesTransferred { get; set; }
        public long TotalBytes { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsUpload { get; set; }
    }

    public class FtpErrorEventArgs : EventArgs
    {
        public string FileName { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
    #endregion
}

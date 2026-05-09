using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GSP
{
    public class Algorithm
    {

        public  void CalculateRotationOffset(
        double x, double y,
        double centerX, double centerY,
        double angleDegrees, out double deltaX, out double deltaY)
        {
            // 将角度转换为弧度
            double theta = angleDegrees * Math.PI / 180.0;
            double cosTheta = Math.Cos(theta);
            double sinTheta = Math.Sin(theta);

            // 计算平移后的坐标
            double translatedX = x - centerX;
            double translatedY = y - centerY;

            // 计算偏移量
            deltaX = translatedX * (cosTheta - 1) - translatedY * sinTheta;
            deltaY = translatedX * sinTheta + translatedY * (cosTheta - 1);
        }
        /// <summary>
        /// 旋转平移算法
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="rx">旋转中心X值</param>
        /// <param name="ry">旋转中心Y值</param>
        /// <param name="x1">输出X</param>
        /// <param name="y1">输出Y</param>
        public void OffsetRotation_XY(double x, double y, double a, double rx, double ry, out double x1, out double y1)
        {
            x1 = (x - rx) * Math.Cos(a * Math.PI / 180) - (y - ry) * Math.Sin(a * Math.PI / 180) + rx;

            y1 = (x - rx) * Math.Sin(a * Math.PI / 180) + (y - ry) * Math.Cos(a * Math.PI / 180) + ry;
        }
        /// <summary>
        /// 计算直线长度
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="xe"></param>
        /// <param name="ye"></param>
        /// <returns></returns>
        public double GetLong(double Start_X, double Start_Y, double End_X, double End_Y)
        {
            double Dis_X = End_X - Start_X;
            double Dis_Y = End_Y - Start_Y;
            double d = (Dis_X * Dis_X) + (Dis_Y * Dis_Y);
            return Math.Sqrt(d);
        }
        /// <summary>
        /// 获取两点夹角
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="X1"></param>
        /// <param name="Y1"></param>
        /// <returns></returns>
        public double GetAngle(double X, double Y, double X1, double Y1)
        {
            double D = (Y1 - Y) / (X1 - X);
            return Math.Round(Math.Atan(D) * 180 / Math.PI, 4);
        }

        public double GetAngle(double angle)
        {
            double _angle = angle;
            if (angle == 180 || angle == 360)
            {
                _angle = 0;
            }
            else if (angle > 90 && angle < 180)
            {
                _angle = angle - 180;
            }
            else if (angle > 180 && angle < 270)
            {
                _angle = angle - 180;
            }
            else if (angle > 270 && angle < 360)
            {
                _angle = angle - 360;
            }
            else if (angle == 270)
            {
                _angle = 90;
            }
            return _angle;
        }

        public double GetAngle(double angle, double Dangle)
        {
            angle = angle + Dangle;
            double _angle = angle;

            if (angle == 180 || angle == 360)
            {
                _angle = 0;
            }
            else if (angle > 90 && angle < 180)
            {
                _angle = angle - 180;
            }
            else if (angle > 180 && angle < 270)
            {
                _angle = angle - 180;
            }
            else if (angle > 270 && angle < 360)
            {
                _angle = angle - 360;
            }
            else if (angle == 270)
            {
                _angle = 90;
            }
            else if (angle > 360 && angle < 360 + Dangle)
            {
                _angle = Dangle;
            }
            return _angle;
        }

        /// <summary>
        /// 计算像素比值
        /// </summary>
        /// <param name="PixX"></param>
        /// <param name="PixY"></param>
        /// <param name="XPos"></param>
        /// <param name="YPos"></param>
        /// <returns></returns>
        public double PosToPix(double[] PixX, double[] PixY, double[] XPos, double[] YPos)
        {
            double pix = 0;
            for (int i = 2; i < 9; i += 2)
            {
                pix += (GetLong(XPos[0], YPos[0], XPos[i], YPos[i]) / GetLong(PixX[0], PixY[0], PixY[i], PixX[i]));
            }
            return pix / 4;
        }
        #region 拼图算法
        public string[] Photolocation(int Model, double StartX, double StartY, double PcbWight, double PcbHight, double ImageWight, double ImageHight, out double[] X, out double[] Y, out double Rows, out double Cols)
        {
            double cols = PcbWight / ImageWight;
            double rows = PcbHight / ImageHight;

            double _Cols = PcbWight % ImageWight;
            double _Rows = PcbHight % ImageHight;

            Rows = Math.Ceiling(rows); 
            Cols = Math.Ceiling(cols);

            X = new double[(int)(Cols * Rows) + 1]; Y = new double[(int)(Cols * Rows) + 1];

            string[] mes = new string[(int)(Cols * Rows)];

            int n = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    n++;
                    switch (Model)
                    {
                        case 0:
                            X[n] = StartX + ImageWight * j;
                            Y[n] = StartY + ImageHight * i;
                            break;
                        case 1:
                            X[n] = StartX + ImageWight * j;
                            Y[n] = StartY - ImageHight * i;
                            break;
                        case 2:
                        case 3:
                            X[n] = StartX - ImageWight * j;
                            Y[n] = StartY + ImageHight * i;
                            break;
                    }
                    string str = string.Format("{0}行{1}列", (i + 1).ToString(), (j + 1).ToString());
                    mes[n - 1] = str;
                }
            }
            return mes;
        }
        public string[] Photolocation2(int Model, double StartX, double StartY, double ImageWight, double ImageHight, out double[] X, out double[] Y, double Rows, double Cols)
        {
            X = new double[(int)(Cols * Rows) + 1]; Y = new double[(int)(Cols * Rows) + 1];

            string[] mes = new string[(int)(Cols * Rows)];

            int n = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    n++;
                    switch (Model)
                    {
                        case 0:
                            if (i == 0 && j == 0)
                            {
                                X[n] = StartX + ImageWight * j;
                                Y[n] = StartY + ImageHight * i;
                            }
                            else
                            {
                                X[n] = StartX + ImageWight * j - i*0;
                                Y[n] = StartY + ImageHight * i + j*0;
                            }

                            break;
                        case 1:
                            if (i == 0 && j == 0)
                            {
                                X[n] = StartX + ImageWight * j;
                                Y[n] = StartY - ImageHight * i;
                            }
                            else
                            {
                                X[n] = StartX + ImageWight * j - i * 0;
                                Y[n] = StartY - ImageHight * i + j * 0;
                            }
                            break;
                        case 2:
                        case 3:
                            if (i == 0 && j == 0)
                            {
                                X[n] = StartX - ImageWight * j;
                                Y[n] = StartY + ImageHight * i;
                            }
                            else
                            {
                                X[n] = StartX - ImageWight * j - i * 0;
                                Y[n] = StartY + ImageHight * i + j * 0;
                            }

                            break;
                    }
                    string str = string.Format("{0}行{1}列", (i + 1).ToString(), (j + 1).ToString());
                    mes[n - 1] = str;
                }
            }
            return mes;
        }
        public void GetStartCenter_Pix(double PcbWight, double PcbHight, double ImageWight, double ImageHight, double I_Wight, double I_Hight, out double i_Wight, out double i_Hight)
        {

            double cols = PcbWight / ImageWight;
            double rows = PcbHight / ImageHight;

            double Rows = Math.Ceiling(rows);
            double Cols = Math.Ceiling(cols);

            i_Wight = I_Wight / Cols;
            i_Hight = I_Hight / Rows;
        }
        public void GetpatePoint(double CentralpointX, double CentralpointY, double ImageWight, double ImageHight, double Wight, double Hight, out double[] X, out double[] Y, out double Rows, out double Cols)
        {
            double cols = Wight / ImageWight;
            double rows = Hight / ImageHight;
            double StartX, StartY;
            Rows = Math.Ceiling(rows);
            Cols = Math.Ceiling(cols);
            if (Rows == 1)
            {
                StartY = CentralpointY;
            }
            else
            {
                StartY = CentralpointY - (double)(Hight / 2);
            }
            if (Cols == 1)
            {
                StartX = CentralpointX;
            }
            else
            {
                StartX = CentralpointX - (double)(Wight / 2);
            }
            X = new double[(int)(Cols * Rows) + 1]; Y = new double[(int)(Cols * Rows) + 1];
            int n = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    n++;
                    X[n] = StartX + ImageWight * j;
                    Y[n] = StartY + ImageHight * i;
                }
            }
        }

        public void GetpatePoint2(double CentralpointX, double CentralpointY, double ImageWight, double ImageHight, double Wight, double Hight, out double[] X, out double[] Y, out double Rows, out double Cols)
        {
            double cols = Wight / ImageWight;
            double rows = Hight / ImageHight;
            double StartX, StartY;
            Rows = Math.Ceiling(rows);
            Cols = Math.Ceiling(cols);
            if (Rows == 1&& Cols == 1)
            {
                StartY = CentralpointY;
                StartX = CentralpointX;
            }
            else
            {
                if (Rows > Cols) { Cols = Rows; }
                else { Rows = Cols; }
                StartX = CentralpointX - (double)(Wight/2);
                StartY = CentralpointY - (double)(Hight/2);
            }
            X = new double[(int)(Cols * Rows) + 1]; Y = new double[(int)(Cols * Rows) + 1];
            int n = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    n++;
                    X[n] = StartX + ImageWight * j;
                    Y[n] = StartY + ImageHight * i;
                }
            }
        }
        #endregion

        #region XY偏差带中心旋转算法
        public int GetOffsetXY(double BX, double BY, double Angle, double CentreX, double CentreY, out double X, out double Y)
        {
            int ErrorCode =0; 
            double dx = 0, dy = 0;
            if (CentreX == 0 && CentreY == 0)
            {
                ErrorCode = 1; X = dx;
                Y = dy; return ErrorCode;
            }
            OffsetRotation_XY(BX, BY, Angle, CentreX, CentreY, out dx, out dy);
            X = dx;
            Y = dy;
            return ErrorCode;
        }

        #endregion

        #region Mask算法
        /// <summary>
        /// 坐标系偏移算法
        /// </summary>
        /// <param name="data"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void CalculateTransferXY(DataTable data, out double X, out double Y)
        {
            double[] BOM_X = (from DataRow dr in data.Rows select Convert.ToDouble(dr["原始X坐标"])).ToArray();
            double[] BOM_Y = (from DataRow dr in data.Rows select Convert.ToDouble(dr["原始Y坐标"])).ToArray();
            double offsetX = BOM_X.Min();
            double offsetY = BOM_Y.Min();
            if (offsetX < 0) { X = Math.Abs(offsetX); }
            else { X = 0; }
            if (offsetY < 0) { Y = Math.Abs(offsetY); }
            else { Y = 0; }
        }
        /// <summary>
        /// Mak计算算法
        /// </summary>
        /// <param name="HomeNum">原点位置0->左下脚 1->右下角 2->右上角 3-左上角 </param>
        /// <param name="bX">原始坐标X</param>
        /// <param name="bY">原始坐标Y</param>
        /// <param name="Mak1_X">纠偏后MAK1_X</param>
        /// <param name="Mak1_Y">纠偏后MAK1_Y</param>
        /// <param name="Mak2_X">纠偏后MAK2_X</param>
        /// <param name="Mak2_Y">纠偏后MAK1_Y</param>
        /// <param name="BMak1_X">原始MAK1_X</param>
        /// <param name="BMak1_Y">原始MAK1_Y</param>
        /// <param name="BMak2_X">原始MAK2_X</param>
        /// <param name="BMak2_Y">原始MAK2_Y</param>
        /// <param name="X">输出位置X</param>
        /// <param name="Y">输出位置Y</param>
        public int MakAlgorithm(int HomeNum, double bX, double bY, double Mak1_X, double Mak1_Y, double Mak2_X, double Mak2_Y, double BMak1_X, double BMak1_Y, double BMak2_X, double BMak2_Y, out double X, out double Y)
        {
            double OffsetX, OffsetY, dAngle;
            double dx = 0, dy = 0;
            int ErrorCode = 0;
            if (BMak1_X == 0 && BMak1_Y == 0 && BMak2_X == 0 && BMak2_Y == 0)
            {
                ErrorCode = 1; X = dx;
                Y = dy; return ErrorCode;
            }
            switch (HomeNum)
            {
                case 0: //待验证
                    OffsetX = Mak1_X + BMak1_X;//计算MAK X偏移
                    OffsetY = Mak1_Y + BMak1_Y;//计算MAK Y偏移
                    double Angle1 = GetAngle(Mak1_X, Mak1_Y, Mak2_X, Mak2_Y);
                    double Angle2 = GetAngle(OffsetX - BMak1_X, OffsetY - BMak1_Y, OffsetX - BMak2_X, OffsetY - BMak2_Y);
                    dAngle = Angle1 - Angle2; //计算角度；
                    bX = Math.Abs(bX);
                    bY = Math.Abs(bY);
                    bX = OffsetX - bX;
                    bY = OffsetY - bY;
                    OffsetRotation_XY(bX, bY, dAngle, Mak1_X, Mak1_Y, out dx, out dy);
                    ErrorCode = 0;
                    break;
                case 4:
                    OffsetX = Mak1_X + BMak1_X;//计算MAK X偏移
                    OffsetY = Mak1_Y + BMak1_Y;//计算MAK Y偏移
                    //计算角度
                    dAngle = GetAngle(Mak1_X, Mak1_Y, Mak2_X, Mak2_Y) - GetAngle(OffsetX - BMak1_X, OffsetY - BMak1_Y, OffsetX - BMak2_X, OffsetY - BMak2_Y);
                    bX = Math.Abs(bX);
                    bY = Math.Abs(bY);
                    bX = OffsetX - bX;
                    bY = OffsetY - bY;
                    OffsetRotation_XY(bX, bY, dAngle, Mak1_X, Mak1_Y, out dx, out dy);
                    ErrorCode = 0;
                    break;
                case 1:  //OK
                    OffsetX = Mak1_X - BMak1_X;//计算MAK X偏移
                    OffsetY = Mak1_Y + BMak1_Y;//计算MAK Y偏移
                    dAngle = GetAngle(Mak1_X, Mak1_Y, Mak2_X, Mak2_Y) - GetAngle(BMak1_X + OffsetX, OffsetY - BMak1_Y, BMak2_X + OffsetX, OffsetY - BMak2_Y); //计算角度；
                    bX = Math.Abs(bX);
                    bY = Math.Abs(bY);
                    bX = OffsetX + bX;
                    bY = OffsetY - bY;
                    OffsetRotation_XY(bX, bY, dAngle, Mak1_X, Mak1_Y, out dx, out dy);
                    break;
                case 2: //待验证
                    OffsetX = Mak1_X - BMak1_X;//计算MAK X偏移
                    OffsetY = Mak1_Y - BMak1_Y;//计算MAK Y偏移
                    dAngle = GetAngle(Mak1_X, Mak1_Y, Mak2_X, Mak2_Y) - GetAngle(OffsetX + BMak1_X, OffsetY + BMak1_Y, OffsetX + BMak2_X, OffsetY + BMak2_Y); //计算角度；
                    bX = Math.Abs(bX);
                    bY = Math.Abs(bY);
                    bX = OffsetX + bX;
                    bY = OffsetY + bY;
                    OffsetRotation_XY(bX, bY, dAngle, Mak1_X, Mak1_Y, out dx, out dy);
                    break;
                case 3:
                    OffsetX = Mak1_X + BMak1_X;//计算MAK X偏移
                    OffsetY = Mak1_Y - BMak1_Y;//计算MAK Y偏移
                    dAngle = GetAngle(Mak1_X, Mak1_Y, Mak2_X, Mak2_Y) - GetAngle(OffsetX - BMak1_X, OffsetY + BMak1_Y, OffsetX - BMak2_X, OffsetY + BMak2_Y); //计算角度；
                    bX = Math.Abs(bX);
                    bY = Math.Abs(bY);
                    bX = OffsetX - bX;
                    bY = OffsetY + bY;
                    OffsetRotation_XY(bX, bY, dAngle, Mak1_X, Mak1_Y, out dx, out dy);
                    break;

            }
            X = dx;
            Y = dy;
            return ErrorCode;
        }

        public int MakAlgorithm(double Rote, double bX, double bY, double Mak1_X, double Mak1_Y, double Mak2_X, double Mak2_Y, double BMak1_X, double BMak1_Y, double BMak2_X, double BMak2_Y, out double X, out double Y)
        {
            double OffsetX, OffsetY, dAngle;
            double dx = 0, dy = 0, Rdx = 0, Rdy = 0;
            int ErrorCode = 0;
            if (BMak1_X == 0 && BMak1_Y == 0 && BMak2_X == 0 && BMak2_Y == 0)
            {
                ErrorCode = 1; X = dx;
                Y = dy; return ErrorCode;
            }
            OffsetX = Mak1_X + BMak1_X;//计算MAK X偏移
            OffsetY = Mak1_Y + BMak1_Y;//计算MAK Y偏移
            double Angle1 = GetAngle(Mak1_X, Mak1_Y, Mak2_X, Mak2_Y);
            double Angle2 = GetAngle(OffsetX - BMak1_X, OffsetY - BMak1_Y, OffsetX - BMak2_X, OffsetY - BMak2_Y);
            dAngle = Angle1 - Angle2; //计算角度；
            bX = Math.Abs(bX);
            bY = Math.Abs(bY);
            bX = OffsetX - bX;
            bY = OffsetY - bY;
            OffsetRotation_XY(Mak2_X, Mak2_Y, Rote, Mak1_X, Mak1_Y, out Rdx, out Rdy);
            double Trdx = Rdx - Mak2_X;
            double Trdy = Rdy - Mak2_Y;
            OffsetRotation_XY(bX, bY, Rote - dAngle, Mak1_X, Mak1_Y, out dx, out dy);
            ErrorCode = 0;
            X = dx - Trdx;
            Y = dy - Trdy;
            return ErrorCode;
        }

        private static readonly Random _globalRandom = new Random();
        [ThreadStatic] private static Random _localRandom;

        /// <summary>
        /// 线程安全的随机浮点数生成方法
        /// </summary>
        /// 
        public static double RandomDouble(double min, double max,bool check)
        {
            if(check)
            {
                return 0;
            }
            else
            {
                if (_localRandom == null)
                {
                    lock (_globalRandom)
                    {
                        _localRandom = new Random(_globalRandom.Next());
                    }
                }
                return min + _localRandom.NextDouble() * (max - min);
            }
        }
        #endregion
    }


}

using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP_SJ.ModelClass
{
    public class DeepOCRHelper
    {

        // Procedures 
        // External procedures 
        // Chapter: OCR / Deep OCR
        // Short Description: Visualize Deep OCR detection and recognition results. 
        public void dev_display_deep_ocr_results(HObject ho_Image, HTuple hv_WindowHandle,
            HTuple hv_DeepOcrResult, HTuple hv_GenParamName, HTuple hv_GenParamValue)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_WordRectangle = null, ho_Arrow = null;

            // Local copy input parameter variables 
            HObject ho_Image_COPY_INP_TMP;
            ho_Image_COPY_INP_TMP = new HObject(ho_Image);



            // Local control variables 

            HTuple hv_NumberIconicObjects = new HTuple();
            HTuple hv_BoxColor = new HTuple(), hv_LineWidth = new HTuple();
            HTuple hv_FontSize = new HTuple(), hv_ShowWords = new HTuple();
            HTuple hv_ShowOrientation = new HTuple(), hv_ActiveWindow = new HTuple();
            HTuple hv_DetectionMode = new HTuple(), hv_RecognitionMode = new HTuple();
            HTuple hv_Words = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Col = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_ArrowSizeFactorLength = new HTuple();
            HTuple hv_ArrowSizeFactorHead = new HTuple(), hv_MaxLengthArrow = new HTuple();
            HTuple hv_HalfLengthArrow = new HTuple(), hv_ArrowBaseRow = new HTuple();
            HTuple hv_ArrowBaseCol = new HTuple(), hv_ArrowHeadRow = new HTuple();
            HTuple hv_ArrowHeadCol = new HTuple(), hv_ArrowHeadSize = new HTuple();
            HTuple hv_HasRecognition = new HTuple(), hv_RecognizedWord = new HTuple();
            HTuple hv_D = new HTuple(), hv_Alpha = new HTuple(), hv_WordRow = new HTuple();
            HTuple hv_WordCol = new HTuple(), hv__ = new HTuple();
            HTuple hv_StringWidth = new HTuple(), hv_StringHeight = new HTuple();
            HTuple hv_MarginBottom = new HTuple(), hv_WindowWidth = new HTuple();
            HTuple hv_WindowHeight = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ZoomedImageHeight = new HTuple();
            HTuple hv_ZoomedImageWidth = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_WordRectangle);
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            try
            {
                //This procedure visualizes the results DeepOcrResult of a Deep OCR model.
                //
                //
                hv_NumberIconicObjects.Dispose();
                HOperatorSet.CountObj(ho_Image_COPY_INP_TMP, out hv_NumberIconicObjects);
                if ((int)(new HTuple(hv_NumberIconicObjects.TupleGreater(1))) != 0)
                {
                    throw new HalconException("Only single input image allowed.");
                }
                //
                if ((int)(new HTuple((new HTuple(hv_DeepOcrResult.TupleLength())).TupleGreater(
                    1))) != 0)
                {
                    throw new HalconException("Only single result dictionary allowed.");
                }
                //
                //Parse generic visualization parameters.
                hv_BoxColor.Dispose(); hv_LineWidth.Dispose(); hv_FontSize.Dispose(); hv_ShowWords.Dispose(); hv_ShowOrientation.Dispose();
                parse_generic_visualization_parameters(hv_GenParamName, hv_GenParamValue, out hv_BoxColor,
                    out hv_LineWidth, out hv_FontSize, out hv_ShowWords, out hv_ShowOrientation);
                //
                //Prepare the window.
                //if (HDevWindowStack.IsOpen())
                //{
                //    hv_ActiveWindow = HDevWindowStack.GetActive();
                //}
                //if ((int)(new HTuple(hv_ActiveWindow.TupleNotEqual(hv_WindowHandle))) != 0)
                //{
                //    HDevWindowStack.SetActive(hv_WindowHandle);
                //}
                ////HOperatorSet.SetWindowParam(hv_WindowHandle, "flush", "false");
                //using (HDevDisposeHelper dh = new HDevDisposeHelper())
                //{
                //    HOperatorSet.SetFont(hv_WindowHandle, "default-Normal-" + hv_FontSize);
                //}
                //if (HDevWindowStack.IsOpen())
                //{
                //    HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
                //}
                //
                //Check whether detection and recognition results are available.
                hv_DetectionMode.Dispose();
                HOperatorSet.GetDictParam(hv_DeepOcrResult, "key_exists", "words", out hv_DetectionMode);
                hv_RecognitionMode.Dispose();
                HOperatorSet.GetDictParam(hv_DeepOcrResult, "key_exists", "word", out hv_RecognitionMode);
                //
                if ((int)(hv_DetectionMode) != 0)
                {
                    //Visualize the oriented rectangles marking the detected words.
                    //
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_Image_COPY_INP_TMP, HDevWindowStack.GetActive()
                            );
                    }
                    //
                    //Get rectangle2 boxes of detected words.
                    hv_Words.Dispose();
                    HOperatorSet.GetDictTuple(hv_DeepOcrResult, "words", out hv_Words);
                    hv_Row.Dispose();
                    HOperatorSet.GetDictTuple(hv_Words, "row", out hv_Row);
                    hv_Col.Dispose();
                    HOperatorSet.GetDictTuple(hv_Words, "col", out hv_Col);
                    hv_Phi.Dispose();
                    HOperatorSet.GetDictTuple(hv_Words, "phi", out hv_Phi);
                    hv_Length1.Dispose();
                    HOperatorSet.GetDictTuple(hv_Words, "length1", out hv_Length1);
                    hv_Length2.Dispose();
                    HOperatorSet.GetDictTuple(hv_Words, "length2", out hv_Length2);
                    //
                    if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
                    {
                        //Generate XLD contours of the word rectangles.
                        ho_WordRectangle.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_WordRectangle, hv_Row, hv_Col,
                            hv_Phi, hv_Length1, hv_Length2);
                        //
                        if ((int)(hv_ShowOrientation) != 0)
                        {
                            //Generate orientation arrows.
                            hv_ArrowSizeFactorLength.Dispose();
                            hv_ArrowSizeFactorLength = 0.4;
                            hv_ArrowSizeFactorHead.Dispose();
                            hv_ArrowSizeFactorHead = 0.2;
                            hv_MaxLengthArrow.Dispose();
                            hv_MaxLengthArrow = 20;
                            hv_HalfLengthArrow.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_HalfLengthArrow = hv_MaxLengthArrow.TupleMin2(
                                    hv_Length1 * hv_ArrowSizeFactorLength);
                            }
                            hv_ArrowBaseRow.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ArrowBaseRow = hv_Row - ((hv_Length1 - hv_HalfLengthArrow) * (hv_Phi.TupleSin()
                                    ));
                            }
                            hv_ArrowBaseCol.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ArrowBaseCol = hv_Col + ((hv_Length1 - hv_HalfLengthArrow) * (hv_Phi.TupleCos()
                                    ));
                            }
                            hv_ArrowHeadRow.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ArrowHeadRow = hv_Row - ((hv_Length1 + hv_HalfLengthArrow) * (hv_Phi.TupleSin()
                                    ));
                            }
                            hv_ArrowHeadCol.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ArrowHeadCol = hv_Col + ((hv_Length1 + hv_HalfLengthArrow) * (hv_Phi.TupleCos()
                                    ));
                            }
                            hv_ArrowHeadSize.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ArrowHeadSize = (hv_MaxLengthArrow.TupleMin2(
                                    hv_Length1.TupleMin2(hv_Length2))) * hv_ArrowSizeFactorHead;
                            }
                            ho_Arrow.Dispose();
                            gen_arrow_contour_xld(out ho_Arrow, hv_ArrowBaseRow, hv_ArrowBaseCol,
                                hv_ArrowHeadRow, hv_ArrowHeadCol, hv_ArrowHeadSize, hv_ArrowHeadSize);
                        }
                        //
                        //Display black boundaries around rectangles and arrows.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetColor(HDevWindowStack.GetActive(), "black");
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidth + 2);
                            }
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_WordRectangle, HDevWindowStack.GetActive());
                        }
                        if ((int)(hv_ShowOrientation) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_Arrow, HDevWindowStack.GetActive());
                            }
                        }
                        //
                        //Display rectangles and arrows.
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetColor(HDevWindowStack.GetActive(), hv_BoxColor);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.SetLineWidth(HDevWindowStack.GetActive(), hv_LineWidth);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_WordRectangle, HDevWindowStack.GetActive());
                        }
                        if ((int)(hv_ShowOrientation) != 0)
                        {
                            if (HDevWindowStack.IsOpen())
                            {
                                HOperatorSet.DispObj(ho_Arrow, HDevWindowStack.GetActive());
                            }
                        }
                        //
                        //Display recognized words, if available.
                        if ((int)(hv_ShowWords) != 0)
                        {
                            //
                            //Check whether the result contains recognized words.
                            hv_HasRecognition.Dispose();
                            HOperatorSet.GetDictParam(hv_Words, "key_exists", "word", out hv_HasRecognition);
                            if ((int)(hv_HasRecognition) != 0)
                            {
                                hv_RecognizedWord.Dispose();
                                HOperatorSet.GetDictTuple(hv_Words, "word", out hv_RecognizedWord);
                                //
                                //Display each recognized word at the bottom-left corner of the respective word box.
                                if ((int)(new HTuple((new HTuple(hv_RecognizedWord.TupleLength())).TupleGreater(
                                    0))) != 0)
                                {
                                    hv_D.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_D = hv_Length1.TupleHypot(
                                            hv_Length2);
                                    }
                                    hv_Alpha.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_Alpha = hv_Length2.TupleAtan2(
                                            hv_Length1);
                                    }
                                    hv_WordRow.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_WordRow = hv_Row + ((((hv_Alpha + hv_Phi)).TupleSin()
                                            ) * hv_D);
                                    }
                                    hv_WordCol.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_WordCol = hv_Col - ((((hv_Alpha + hv_Phi)).TupleCos()
                                            ) * hv_D);
                                    }
                                    if (HDevWindowStack.IsOpen())
                                    {
                                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_RecognizedWord,
                                            "image", hv_WordRow, hv_WordCol, "white", (new HTuple("box_color")).TupleConcat(
                                            "shadow"), (new HTuple("black")).TupleConcat("false"));
                                    }
                                }
                            }
                        }
                    }
                }
                else if ((int)(hv_RecognitionMode) != 0)
                {
                    //Recognition mode: Show the recognized word.
                    //
                    hv_RecognizedWord.Dispose();
                    HOperatorSet.GetDictTuple(hv_DeepOcrResult, "word", out hv_RecognizedWord);
                    if ((int)(new HTuple((new HTuple(hv_RecognizedWord.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        throw new HalconException("In recognition mode only a single word can be processed.");
                    }
                    //
                    hv__.Dispose(); hv__.Dispose(); hv_StringWidth.Dispose(); hv_StringHeight.Dispose();
                    HOperatorSet.GetStringExtents(hv_WindowHandle, hv_RecognizedWord, out hv__,
                        out hv__, out hv_StringWidth, out hv_StringHeight);
                    hv_MarginBottom.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MarginBottom = hv_StringHeight + 5;
                    }
                    //
                    //Zoom the image to fit into the window.
                    hv__.Dispose(); hv__.Dispose(); hv_WindowWidth.Dispose(); hv_WindowHeight.Dispose();
                    HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv__, out hv__, out hv_WindowWidth,
                        out hv_WindowHeight);
                    hv_ImageWidth.Dispose(); hv_ImageHeight.Dispose();
                    HOperatorSet.GetImageSize(ho_Image_COPY_INP_TMP, out hv_ImageWidth, out hv_ImageHeight);
                    hv_ZoomedImageHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomedImageHeight = (((new HTuple(0)).TupleMax2(
                            hv_WindowHeight - hv_MarginBottom))).TupleInt();
                    }
                    hv_ZoomedImageWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomedImageWidth = (((hv_ImageWidth / (hv_ImageHeight.TupleReal()
                            )) * hv_ZoomedImageHeight)).TupleInt();
                    }
                    if ((int)(new HTuple(hv_ZoomedImageWidth.TupleGreater(hv_WindowWidth))) != 0)
                    {
                        hv_ZoomedImageWidth.Dispose();
                        hv_ZoomedImageWidth = new HTuple(hv_WindowWidth);
                        hv_ZoomedImageHeight.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ZoomedImageHeight = (((hv_ImageHeight / (hv_ImageWidth.TupleReal()
                                )) * hv_ZoomedImageWidth)).TupleInt();
                        }
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Image_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ZoomedImageWidth,
                            hv_ZoomedImageHeight, "constant");
                        ho_Image_COPY_INP_TMP.Dispose();
                        ho_Image_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    //
                    //Display the image.
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.SetPart(HDevWindowStack.GetActive(), 0, 0, hv_WindowHeight,
                            hv_WindowWidth);
                    }
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_Image_COPY_INP_TMP, HDevWindowStack.GetActive()
                            );
                    }
                    //
                    //Display the recognized word below the image.
                    if ((int)(hv_ShowWords) != 0)
                    {
                        if (HDevWindowStack.IsOpen())
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_RecognizedWord,
                                    "image", hv_WindowHeight - hv_MarginBottom, 5, "white", "box", "false");
                            }
                        }
                    }
                }
                else
                {
                    //Only display the image.
                    if (HDevWindowStack.IsOpen())
                    {
                        HOperatorSet.DispObj(ho_Image_COPY_INP_TMP, HDevWindowStack.GetActive()
                            );
                    }
                }
                //
                //Update window.
                HOperatorSet.SetWindowParam(hv_WindowHandle, "flush", "true");
                HOperatorSet.FlushBuffer(hv_WindowHandle);
                //
                //Restore the originally active window.
                if ((int)(new HTuple(hv_ActiveWindow.TupleNotEqual(hv_WindowHandle))) != 0)
                {
                    HDevWindowStack.SetActive(hv_ActiveWindow);
                }
                //
                ho_Image_COPY_INP_TMP.Dispose();
                ho_WordRectangle.Dispose();
                ho_Arrow.Dispose();

                hv_NumberIconicObjects.Dispose();
                hv_BoxColor.Dispose();
                hv_LineWidth.Dispose();
                hv_FontSize.Dispose();
                hv_ShowWords.Dispose();
                hv_ShowOrientation.Dispose();
                hv_ActiveWindow.Dispose();
                hv_DetectionMode.Dispose();
                hv_RecognitionMode.Dispose();
                hv_Words.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_ArrowSizeFactorLength.Dispose();
                hv_ArrowSizeFactorHead.Dispose();
                hv_MaxLengthArrow.Dispose();
                hv_HalfLengthArrow.Dispose();
                hv_ArrowBaseRow.Dispose();
                hv_ArrowBaseCol.Dispose();
                hv_ArrowHeadRow.Dispose();
                hv_ArrowHeadCol.Dispose();
                hv_ArrowHeadSize.Dispose();
                hv_HasRecognition.Dispose();
                hv_RecognizedWord.Dispose();
                hv_D.Dispose();
                hv_Alpha.Dispose();
                hv_WordRow.Dispose();
                hv_WordCol.Dispose();
                hv__.Dispose();
                hv_StringWidth.Dispose();
                hv_StringHeight.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ZoomedImageHeight.Dispose();
                hv_ZoomedImageWidth.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image_COPY_INP_TMP.Dispose();
                ho_WordRectangle.Dispose();
                ho_Arrow.Dispose();

                hv_NumberIconicObjects.Dispose();
                hv_BoxColor.Dispose();
                hv_LineWidth.Dispose();
                hv_FontSize.Dispose();
                hv_ShowWords.Dispose();
                hv_ShowOrientation.Dispose();
                hv_ActiveWindow.Dispose();
                hv_DetectionMode.Dispose();
                hv_RecognitionMode.Dispose();
                hv_Words.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_ArrowSizeFactorLength.Dispose();
                hv_ArrowSizeFactorHead.Dispose();
                hv_MaxLengthArrow.Dispose();
                hv_HalfLengthArrow.Dispose();
                hv_ArrowBaseRow.Dispose();
                hv_ArrowBaseCol.Dispose();
                hv_ArrowHeadRow.Dispose();
                hv_ArrowHeadCol.Dispose();
                hv_ArrowHeadSize.Dispose();
                hv_HasRecognition.Dispose();
                hv_RecognizedWord.Dispose();
                hv_D.Dispose();
                hv_Alpha.Dispose();
                hv_WordRow.Dispose();
                hv_WordCol.Dispose();
                hv__.Dispose();
                hv_StringWidth.Dispose();
                hv_StringHeight.Dispose();
                hv_MarginBottom.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ZoomedImageHeight.Dispose();
                hv_ZoomedImageWidth.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: XLD / Creation
        // Short Description: Creates an arrow shaped XLD contour. 
        public void gen_arrow_contour_xld(out HObject ho_Arrow, HTuple hv_Row1, HTuple hv_Column1,
            HTuple hv_Row2, HTuple hv_Column2, HTuple hv_HeadLength, HTuple hv_HeadWidth)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TempArrow = null;

            // Local control variables 

            HTuple hv_Length = new HTuple(), hv_ZeroLengthIndices = new HTuple();
            HTuple hv_DR = new HTuple(), hv_DC = new HTuple(), hv_HalfHeadWidth = new HTuple();
            HTuple hv_RowP1 = new HTuple(), hv_ColP1 = new HTuple();
            HTuple hv_RowP2 = new HTuple(), hv_ColP2 = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Arrow);
            HOperatorSet.GenEmptyObj(out ho_TempArrow);
            try
            {
                //This procedure generates arrow shaped XLD contours,
                //pointing from (Row1, Column1) to (Row2, Column2).
                //If starting and end point are identical, a contour consisting
                //of a single point is returned.
                //
                //input parameteres:
                //Row1, Column1: Coordinates of the arrows' starting points
                //Row2, Column2: Coordinates of the arrows' end points
                //HeadLength, HeadWidth: Size of the arrow heads in pixels
                //
                //output parameter:
                //Arrow: The resulting XLD contour
                //
                //The input tuples Row1, Column1, Row2, and Column2 have to be of
                //the same length.
                //HeadLength and HeadWidth either have to be of the same length as
                //Row1, Column1, Row2, and Column2 or have to be a single element.
                //If one of the above restrictions is violated, an error will occur.
                //
                //
                //Init
                ho_Arrow.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Arrow);
                //
                //Calculate the arrow length
                hv_Length.Dispose();
                HOperatorSet.DistancePp(hv_Row1, hv_Column1, hv_Row2, hv_Column2, out hv_Length);
                //
                //Mark arrows with identical start and end point
                //(set Length to -1 to avoid division-by-zero exception)
                hv_ZeroLengthIndices.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ZeroLengthIndices = hv_Length.TupleFind(
                        0);
                }
                if ((int)(new HTuple(hv_ZeroLengthIndices.TupleNotEqual(-1))) != 0)
                {
                    if (hv_Length == null)
                        hv_Length = new HTuple();
                    hv_Length[hv_ZeroLengthIndices] = -1;
                }
                //
                //Calculate auxiliary variables.
                hv_DR.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DR = (1.0 * (hv_Row2 - hv_Row1)) / hv_Length;
                }
                hv_DC.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DC = (1.0 * (hv_Column2 - hv_Column1)) / hv_Length;
                }
                hv_HalfHeadWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HalfHeadWidth = hv_HeadWidth / 2.0;
                }
                //
                //Calculate end points of the arrow head.
                hv_RowP1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RowP1 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) + (hv_HalfHeadWidth * hv_DC);
                }
                hv_ColP1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColP1 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) - (hv_HalfHeadWidth * hv_DR);
                }
                hv_RowP2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RowP2 = (hv_Row1 + ((hv_Length - hv_HeadLength) * hv_DR)) - (hv_HalfHeadWidth * hv_DC);
                }
                hv_ColP2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ColP2 = (hv_Column1 + ((hv_Length - hv_HeadLength) * hv_DC)) + (hv_HalfHeadWidth * hv_DR);
                }
                //
                //Finally create output XLD contour for each input point pair
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_Length.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(new HTuple(((hv_Length.TupleSelect(hv_Index))).TupleEqual(-1))) != 0)
                    {
                        //Create_ single points for arrows with identical start and end point
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_TempArrow.Dispose();
                            HOperatorSet.GenContourPolygonXld(out ho_TempArrow, hv_Row1.TupleSelect(
                                hv_Index), hv_Column1.TupleSelect(hv_Index));
                        }
                    }
                    else
                    {
                        //Create arrow contour
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_TempArrow.Dispose();
                            HOperatorSet.GenContourPolygonXld(out ho_TempArrow, ((((((((((hv_Row1.TupleSelect(
                                hv_Index))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                                hv_RowP1.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)))).TupleConcat(
                                hv_RowP2.TupleSelect(hv_Index)))).TupleConcat(hv_Row2.TupleSelect(hv_Index)),
                                ((((((((((hv_Column1.TupleSelect(hv_Index))).TupleConcat(hv_Column2.TupleSelect(
                                hv_Index)))).TupleConcat(hv_ColP1.TupleSelect(hv_Index)))).TupleConcat(
                                hv_Column2.TupleSelect(hv_Index)))).TupleConcat(hv_ColP2.TupleSelect(
                                hv_Index)))).TupleConcat(hv_Column2.TupleSelect(hv_Index)));
                        }
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_Arrow, ho_TempArrow, out ExpTmpOutVar_0);
                        ho_Arrow.Dispose();
                        ho_Arrow = ExpTmpOutVar_0;
                    }
                }
                ho_TempArrow.Dispose();

                hv_Length.Dispose();
                hv_ZeroLengthIndices.Dispose();
                hv_DR.Dispose();
                hv_DC.Dispose();
                hv_HalfHeadWidth.Dispose();
                hv_RowP1.Dispose();
                hv_ColP1.Dispose();
                hv_RowP2.Dispose();
                hv_ColP2.Dispose();
                hv_Index.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_TempArrow.Dispose();

                hv_Length.Dispose();
                hv_ZeroLengthIndices.Dispose();
                hv_DR.Dispose();
                hv_DC.Dispose();
                hv_HalfHeadWidth.Dispose();
                hv_RowP1.Dispose();
                hv_ColP1.Dispose();
                hv_RowP2.Dispose();
                hv_ColP2.Dispose();
                hv_Index.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Parse generic visualization parameters. 
        private void parse_generic_visualization_parameters(HTuple hv_GenParamName, HTuple hv_GenParamValue,
            out HTuple hv_BoxColor, out HTuple hv_LineWidth, out HTuple hv_FontSize, out HTuple hv_ShowWords,
            out HTuple hv_ShowOrientation)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ShowScoreMaps = new HTuple(), hv_ParamIdx = new HTuple();
            HTuple hv_AllowedGenParams = new HTuple(), hv_CaseIdx = new HTuple();
            HTuple hv_BoolShowWords = new HTuple(), hv_BoolShowArrow = new HTuple();
            // Initialize local and output iconic variables 
            hv_BoxColor = new HTuple();
            hv_LineWidth = new HTuple();
            hv_FontSize = new HTuple();
            hv_ShowWords = new HTuple();
            hv_ShowOrientation = new HTuple();
            try
            {
                //Set default values.
                hv_BoxColor.Dispose();
                hv_BoxColor = "green";
                hv_LineWidth.Dispose();
                hv_LineWidth = 3;
                hv_FontSize.Dispose();
                hv_FontSize = 12;
                hv_ShowScoreMaps.Dispose();
                hv_ShowScoreMaps = 1;
                hv_ShowWords.Dispose();
                hv_ShowWords = 1;
                hv_ShowOrientation.Dispose();
                hv_ShowOrientation = 1;

                //Parse the generic parameters.
                for (hv_ParamIdx = 0; (int)hv_ParamIdx <= (int)((new HTuple(hv_GenParamName.TupleLength()
                    )) - 1); hv_ParamIdx = (int)hv_ParamIdx + 1)
                {
                    hv_AllowedGenParams.Dispose();
                    hv_AllowedGenParams = new HTuple();
                    hv_AllowedGenParams[0] = "box_color";
                    hv_AllowedGenParams[1] = "line_width";
                    hv_AllowedGenParams[2] = "font_size";
                    hv_AllowedGenParams[3] = "show_words";
                    hv_AllowedGenParams[4] = "show_orientation";
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CaseIdx.Dispose();
                        HOperatorSet.TupleFind(hv_AllowedGenParams, hv_GenParamName.TupleSelect(hv_ParamIdx),
                            out hv_CaseIdx);
                    }
                    switch (hv_CaseIdx.I)
                    {
                        case 0:
                            //Get color.
                            hv_BoxColor.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BoxColor = hv_GenParamValue.TupleSelect(
                                    hv_ParamIdx);
                            }
                            break;
                        case 1:
                            //Get line width.
                            hv_LineWidth.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_LineWidth = hv_GenParamValue.TupleSelect(
                                    hv_ParamIdx);
                            }
                            break;
                        case 2:
                            //Get font size.
                            hv_FontSize.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_FontSize = hv_GenParamValue.TupleSelect(
                                    hv_ParamIdx);
                            }
                            break;
                        case 3:
                            //Check whether words shall be displayed.
                            hv_BoolShowWords.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BoolShowWords = hv_GenParamValue.TupleSelect(
                                    hv_ParamIdx);
                            }
                            if ((int)(new HTuple(hv_BoolShowWords.TupleEqual("true"))) != 0)
                            {
                                hv_ShowWords.Dispose();
                                hv_ShowWords = 1;
                            }
                            else
                            {
                                hv_ShowWords.Dispose();
                                hv_ShowWords = 0;
                            }
                            break;
                        case 4:
                            //Check whether arrow of the word box should be displayed.
                            hv_BoolShowArrow.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_BoolShowArrow = hv_GenParamValue.TupleSelect(
                                    hv_ParamIdx);
                            }
                            if ((int)(new HTuple(hv_BoolShowArrow.TupleEqual("true"))) != 0)
                            {
                                hv_ShowOrientation.Dispose();
                                hv_ShowOrientation = 1;
                            }
                            else
                            {
                                hv_ShowOrientation.Dispose();
                                hv_ShowOrientation = 0;
                            }
                            break;
                        case -1:
                            //General parameter not valid.
                            throw new HalconException(("The general parameter \"" + (hv_GenParamName.TupleSelect(
                                hv_ParamIdx))) + "\" is not valid.");
                            break;
                    }
                }

                hv_ShowScoreMaps.Dispose();
                hv_ParamIdx.Dispose();
                hv_AllowedGenParams.Dispose();
                hv_CaseIdx.Dispose();
                hv_BoolShowWords.Dispose();
                hv_BoolShowArrow.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ShowScoreMaps.Dispose();
                hv_ParamIdx.Dispose();
                hv_AllowedGenParams.Dispose();
                hv_CaseIdx.Dispose();
                hv_BoolShowWords.Dispose();
                hv_BoolShowArrow.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
            HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behaviour
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Local procedures 
        public void display_recognition_alphabet(HTuple hv_RecognitionAlphabet, HTuple hv_WindowHandle)
        {
            // Local iconic variables 

            // Local control variables 

            HTuple hv_Text = new HTuple(), hv_Number = new HTuple();
            HTuple hv_Textline = new HTuple(), hv_i = new HTuple();
            HTuple hv_Minor = new HTuple(), hv_Capital = new HTuple();
            HTuple hv_Special = new HTuple(), hv_Line = new HTuple();
            HTuple hv_c = new HTuple(), hv_Char = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_TextWidth = new HTuple();
            HTuple hv_TextHeight = new HTuple(), hv__ = new HTuple();
            HTuple hv_LineWidth = new HTuple(), hv_LineHeight = new HTuple();
            HTuple hv_WindowWidth = new HTuple(), hv_WindowHeight = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //Display the recognition alphabet in the given window handle.
                //
                hv_Text.Dispose();
                hv_Text = "Characters the model can recognize:";
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = "";
                //
                //Sort the characters and set the text to be displayed.
                //1) 0-9
                hv_Number.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Number = ((hv_RecognitionAlphabet.TupleRegexpMatch(
                        "[0-9]"))).TupleUniq();
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Number = hv_Number.TupleRemove(
                            hv_Number.TupleFind(""));
                        hv_Number.Dispose();
                        hv_Number = ExpTmpLocalVar_Number;
                    }
                }
                hv_Textline.Dispose();
                hv_Textline = "";
                for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Number.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Textline = (hv_Textline + (hv_Number.TupleSelect(
                                hv_i))) + " ";
                            hv_Textline.Dispose();
                            hv_Textline = ExpTmpLocalVar_Textline;
                        }
                    }
                }
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_Textline;
                //2) a-z
                hv_Minor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Minor = ((hv_RecognitionAlphabet.TupleRegexpMatch(
                        "[a-z]"))).TupleUniq();
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Minor = hv_Minor.TupleRemove(
                            hv_Minor.TupleFind(""));
                        hv_Minor.Dispose();
                        hv_Minor = ExpTmpLocalVar_Minor;
                    }
                }
                hv_Textline.Dispose();
                hv_Textline = "";
                for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Minor.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Textline = (hv_Textline + (hv_Minor.TupleSelect(
                                hv_i))) + " ";
                            hv_Textline.Dispose();
                            hv_Textline = ExpTmpLocalVar_Textline;
                        }
                    }
                }
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_Textline;
                //3) A-Z
                hv_Capital.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Capital = ((hv_RecognitionAlphabet.TupleRegexpMatch(
                        "[A-Z]"))).TupleUniq();
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Capital = hv_Capital.TupleRemove(
                            hv_Capital.TupleFind(""));
                        hv_Capital.Dispose();
                        hv_Capital = ExpTmpLocalVar_Capital;
                    }
                }
                hv_Textline.Dispose();
                hv_Textline = "";
                for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Capital.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Textline = (hv_Textline + (hv_Capital.TupleSelect(
                                hv_i))) + " ";
                            hv_Textline.Dispose();
                            hv_Textline = ExpTmpLocalVar_Textline;
                        }
                    }
                }
                if (hv_Text == null)
                    hv_Text = new HTuple();
                hv_Text[new HTuple(hv_Text.TupleLength())] = hv_Textline;
                //4) Further characters
                hv_Special.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Special = hv_RecognitionAlphabet.TupleDifference(
                        ((hv_Minor.TupleConcat(hv_Capital))).TupleConcat(hv_Number));
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Special = hv_Special.TupleRemove(
                            hv_Special.TupleFind(""));
                        hv_Special.Dispose();
                        hv_Special = ExpTmpLocalVar_Special;
                    }
                }
                for (hv_Line = 0; (int)hv_Line <= (int)((new HTuple(hv_Special.TupleLength())) - 1); hv_Line = (int)hv_Line + (int)(new HTuple(hv_Capital.TupleLength()
                    )))
                {
                    hv_Textline.Dispose();
                    hv_Textline = "";
                    HTuple end_val35 = (new HTuple(hv_Capital.TupleLength()
                        )).TupleMin2(((new HTuple(hv_Special.TupleLength())) - 1) - hv_Line);
                    HTuple step_val35 = 1;
                    for (hv_c = 0; hv_c.Continue(end_val35, step_val35); hv_c = hv_c.TupleAdd(step_val35))
                    {
                        //Some characters need special treatment for correct display.
                        hv_Char.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Char = hv_Special.TupleSelect(
                                hv_Line + hv_c);
                        }
                        if ((int)(new HTuple(hv_Char.TupleEqual("\n"))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Textline = (hv_Textline + "\\n") + " ";
                                    hv_Textline.Dispose();
                                    hv_Textline = ExpTmpLocalVar_Textline;
                                }
                            }
                        }
                        else if ((int)(new HTuple(hv_Char.TupleEqual("'"))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Textline = (hv_Textline + "\\'") + " ";
                                    hv_Textline.Dispose();
                                    hv_Textline = ExpTmpLocalVar_Textline;
                                }
                            }
                        }
                        else
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Textline = (hv_Textline + hv_Char) + " ";
                                    hv_Textline.Dispose();
                                    hv_Textline = ExpTmpLocalVar_Textline;
                                }
                            }
                        }
                    }
                    if (hv_Text == null)
                        hv_Text = new HTuple();
                    hv_Text[new HTuple(hv_Text.TupleLength())] = hv_Textline;
                }
                //
                //Adapt window size.
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetWindowExtents(hv_WindowHandle, out hv_Row, out hv_Column, out hv_Width,
                    out hv_Height);
                set_display_font(hv_WindowHandle, 16, "mono", "true", "false");
                hv_TextWidth.Dispose();
                hv_TextWidth = -1;
                hv_TextHeight.Dispose();
                hv_TextHeight = -1;
                for (hv_Line = 0; (int)hv_Line <= (int)((new HTuple(hv_Text.TupleLength())) - 1); hv_Line = (int)hv_Line + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv__.Dispose(); hv__.Dispose(); hv_LineWidth.Dispose(); hv_LineHeight.Dispose();
                        HOperatorSet.GetStringExtents(hv_WindowHandle, hv_Text.TupleSelect(hv_Line),
                            out hv__, out hv__, out hv_LineWidth, out hv_LineHeight);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TextWidth = hv_TextWidth.TupleMax2(
                                hv_LineWidth);
                            hv_TextWidth.Dispose();
                            hv_TextWidth = ExpTmpLocalVar_TextWidth;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TextHeight = hv_TextHeight.TupleMax2(
                                hv_LineHeight);
                            hv_TextHeight.Dispose();
                            hv_TextHeight = ExpTmpLocalVar_TextHeight;
                        }
                    }
                }
                hv_WindowWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowWidth = hv_TextWidth + 62;
                }
                hv_WindowHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_WindowHeight = (hv_TextHeight * ((new HTuple(hv_Text.TupleLength()
                        )) + 1)) + 75;
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetWindowExtents(HDevWindowStack.GetActive(), 0, 0, hv_WindowWidth,
                        hv_WindowHeight);
                }
                //
                //Display the text.
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                        "left", "white", "box", "false");
                }
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), "Press Run (F5) to continue",
                        "window", "bottom", "right", "black", "box", "true");
                }

                hv_Text.Dispose();
                hv_Number.Dispose();
                hv_Textline.Dispose();
                hv_i.Dispose();
                hv_Minor.Dispose();
                hv_Capital.Dispose();
                hv_Special.Dispose();
                hv_Line.Dispose();
                hv_c.Dispose();
                hv_Char.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();
                hv__.Dispose();
                hv_LineWidth.Dispose();
                hv_LineHeight.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Text.Dispose();
                hv_Number.Dispose();
                hv_Textline.Dispose();
                hv_i.Dispose();
                hv_Minor.Dispose();
                hv_Capital.Dispose();
                hv_Special.Dispose();
                hv_Line.Dispose();
                hv_c.Dispose();
                hv_Char.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_TextWidth.Dispose();
                hv_TextHeight.Dispose();
                hv__.Dispose();
                hv_LineWidth.Dispose();
                hv_LineHeight.Dispose();
                hv_WindowWidth.Dispose();
                hv_WindowHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Scaling of detections to preprocessed image. 
        public void scale_detections_to_preprocessed_image(out HObject ho_ScaledDetections,
            HTuple hv_DeepOcrResult, HTuple hv_ImageWidth, HTuple hv_ImageHeight, HTuple hv_ModelWidth,
            HTuple hv_ModelHeight, out HTuple hv_WordsDetected)
        {



            // Local iconic variables 

            HObject ho_Detections = null;

            // Local control variables 

            HTuple hv_Words = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Col = new HTuple(), hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_ScaleWidthUnit = new HTuple(), hv_ScaleHeightUnit = new HTuple();
            HTuple hv_ScaleWidth = new HTuple(), hv_ScaleHeight = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_HomMat2D = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ScaledDetections);
            HOperatorSet.GenEmptyObj(out ho_Detections);
            hv_WordsDetected = new HTuple();
            try
            {
                //This procedure maps the rectangles of the localized words
                //from the input image to the preprocessed image.
                //
                //Get detection parameters.
                hv_Words.Dispose();
                HOperatorSet.GetDictTuple(hv_DeepOcrResult, "words", out hv_Words);
                hv_Row.Dispose();
                HOperatorSet.GetDictTuple(hv_Words, "row", out hv_Row);
                hv_Col.Dispose();
                HOperatorSet.GetDictTuple(hv_Words, "col", out hv_Col);
                hv_Phi.Dispose();
                HOperatorSet.GetDictTuple(hv_Words, "phi", out hv_Phi);
                hv_Length1.Dispose();
                HOperatorSet.GetDictTuple(hv_Words, "length1", out hv_Length1);
                hv_Length2.Dispose();
                HOperatorSet.GetDictTuple(hv_Words, "length2", out hv_Length2);
                hv_WordsDetected.Dispose();
                hv_WordsDetected = 1;
                if ((int)((new HTuple((new HTuple((new HTuple((new HTuple(hv_Row.TupleEqual(
                    new HTuple()))).TupleOr(new HTuple(hv_Col.TupleEqual(new HTuple()))))).TupleOr(
                    new HTuple(hv_Phi.TupleEqual(new HTuple()))))).TupleOr(new HTuple(hv_Length1.TupleEqual(
                    new HTuple()))))).TupleOr(new HTuple(hv_Length2.TupleEqual(new HTuple())))) != 0)
                {
                    hv_WordsDetected.Dispose();
                    hv_WordsDetected = 0;
                    ho_Detections.Dispose();

                    hv_Words.Dispose();
                    hv_Row.Dispose();
                    hv_Col.Dispose();
                    hv_Phi.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_Exception.Dispose();
                    hv_ScaleWidthUnit.Dispose();
                    hv_ScaleHeightUnit.Dispose();
                    hv_ScaleWidth.Dispose();
                    hv_ScaleHeight.Dispose();
                    hv_Scale.Dispose();
                    hv_HomMat2D.Dispose();

                    return;
                }
                try
                {
                    ho_Detections.Dispose();
                    HOperatorSet.GenRectangle2ContourXld(out ho_Detections, hv_Row, hv_Col, hv_Phi,
                        hv_Length1, hv_Length2);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_WordsDetected.Dispose();
                    hv_WordsDetected = 0;
                    ho_Detections.Dispose();

                    hv_Words.Dispose();
                    hv_Row.Dispose();
                    hv_Col.Dispose();
                    hv_Phi.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_Exception.Dispose();
                    hv_ScaleWidthUnit.Dispose();
                    hv_ScaleHeightUnit.Dispose();
                    hv_ScaleWidth.Dispose();
                    hv_ScaleHeight.Dispose();
                    hv_Scale.Dispose();
                    hv_HomMat2D.Dispose();

                    return;
                }
                //
                //Get scale factors.
                //Calculate the unit scale factors, which scale the input to 1px.
                hv_ScaleWidthUnit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleWidthUnit = 1.0 / (hv_ImageWidth.TupleReal()
                        );
                }
                hv_ScaleHeightUnit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleHeightUnit = 1.0 / (hv_ImageHeight.TupleReal()
                        );
                }
                //Calculate the required scale factors to scale regions to
                //model dimensions.
                hv_ScaleWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleWidth = hv_ModelWidth * hv_ScaleWidthUnit;
                }
                hv_ScaleHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleHeight = hv_ModelHeight * hv_ScaleHeightUnit;
                }
                //Compute the minimum scale factor to account for the aspect ratio
                //preserving padding.
                hv_Scale.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Scale = hv_ScaleWidth.TupleMin2(
                        hv_ScaleHeight);
                }
                //Ensure that the scale factors result in lengths of at least 1px.
                hv_ScaleWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleWidth = hv_Scale.TupleMax2(
                        hv_ScaleWidthUnit);
                }
                hv_ScaleHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleHeight = hv_Scale.TupleMax2(
                        hv_ScaleHeightUnit);
                }
                //
                hv_HomMat2D.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.HomMat2dScale(hv_HomMat2D, hv_ScaleWidth, hv_ScaleHeight, 0, 0,
                        out ExpTmpOutVar_0);
                    hv_HomMat2D.Dispose();
                    hv_HomMat2D = ExpTmpOutVar_0;
                }
                ho_ScaledDetections.Dispose();
                HOperatorSet.AffineTransContourXld(ho_Detections, out ho_ScaledDetections,
                    hv_HomMat2D);
                //
                ho_Detections.Dispose();

                hv_Words.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Exception.Dispose();
                hv_ScaleWidthUnit.Dispose();
                hv_ScaleHeightUnit.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_Scale.Dispose();
                hv_HomMat2D.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Detections.Dispose();

                hv_Words.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Exception.Dispose();
                hv_ScaleWidthUnit.Dispose();
                hv_ScaleHeightUnit.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_Scale.Dispose();
                hv_HomMat2D.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public static void set_suitable_device_in_ocr_handle(HTuple hv_DeepOcrHandle)
        {



            // Local control variables 

            HTuple hv_DLDeviceHandles = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //Determine deep learning device to work with (prefer GPU over CPU).
                hv_DLDeviceHandles.Dispose();
                HOperatorSet.QueryAvailableDlDevices((new HTuple("runtime")).TupleConcat("runtime"),
                    (new HTuple("gpu")).TupleConcat("cpu"), out hv_DLDeviceHandles);
                if ((int)(new HTuple((new HTuple(hv_DLDeviceHandles.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    throw new HalconException("No supported device found to continue this example.");
                }
                //Set recognition_image_width larger for the example to work without memory problems.
                try
                {
                    HOperatorSet.SetDeepOcrParam(hv_DeepOcrHandle, "recognition_image_width",
                        250);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_DLDeviceHandles.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    try
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDeepOcrParam(hv_DeepOcrHandle, "device", hv_DLDeviceHandles.TupleSelect(
                                hv_Index));
                        }
                        break;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        if ((int)(new HTuple(hv_Index.TupleEqual((new HTuple(hv_DLDeviceHandles.TupleLength()
                            )) - 1))) != 0)
                        {
                            throw new HalconException("Could not set any of the supported devices to continue this example.");
                        }
                    }
                }
                //Reset recognition_image_width to the default value.
                try
                {
                    HOperatorSet.SetDeepOcrParam(hv_DeepOcrHandle, "recognition_image_width",
                        100);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //

                hv_DLDeviceHandles.Dispose();
                hv_Exception.Dispose();
                hv_Index.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_DLDeviceHandles.Dispose();
                hv_Exception.Dispose();
                hv_Index.Dispose();

                throw HDevExpDefaultException;
            }
        }
        static HTuple hv_DeepOcrHandle;

        public static void Init()
        {
            HOperatorSet.CreateDeepOcr(new HTuple(), new HTuple(), out hv_DeepOcrHandle);
            set_suitable_device_in_ocr_handle(hv_DeepOcrHandle);
        }

        public static void FindOCR(HImage ho_Image, out string text)
        {
            text = "";
            HObject ho_ImageWord;//, ho_ImagePreprocessed;
            //HObject ho_ImagePreprocessedPadded;

            HTuple hv_WindowHandle = new HTuple();
            HTuple hv_DeepOcrResult = new HTuple(), hv_WindowHandlePreprocessed1 = new HTuple();
            HTuple hv_RecognitionImageWidth = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageWord);
            //HOperatorSet.GenEmptyObj(out ho_ImagePreprocessed);
            //HOperatorSet.GenEmptyObj(out ho_ImagePreprocessedPadded);
            try
            {
                ho_ImageWord.Dispose();
                ho_ImageWord = ho_Image;

                hv_DeepOcrResult.Dispose();
                HOperatorSet.ApplyDeepOcr(ho_ImageWord, hv_DeepOcrHandle, "auto", out hv_DeepOcrResult);

                //ho_ImagePreprocessed.Dispose();
                //HOperatorSet.GetDictObject(out ho_ImagePreprocessed, hv_DeepOcrResult, "image");
                //ho_ImagePreprocessedPadded.Dispose();
                //HOperatorSet.ChangeFormat(ho_ImagePreprocessed, out ho_ImagePreprocessedPadded, 500, 32);
                HOperatorSet.GetDictTuple(hv_DeepOcrResult, "words", out HTuple hv_Words);
                HOperatorSet.GetDictTuple(hv_Words, "row", out HTuple hv_Row);
                if ((int)(new HTuple((new HTuple(hv_Row.TupleLength())).TupleGreater(0))) != 0)
                {
                    HOperatorSet.GetDictParam(hv_Words, "key_exists", "word", out HTuple hv_HasRecognition);
                    if ((int)(hv_HasRecognition) != 0)
                    {
                        HOperatorSet.GetDictTuple(hv_Words, "word", out HTuple hv_RecognizedWord);
                        text = hv_RecognizedWord.S;
                    }
                }


            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();
                ho_ImageWord.Dispose();
                //ho_ImagePreprocessed.Dispose();
                //ho_ImagePreprocessedPadded.Dispose();

                hv_WindowHandle.Dispose();
                hv_DeepOcrResult.Dispose();
                hv_WindowHandlePreprocessed1.Dispose();
                hv_RecognitionImageWidth.Dispose();

                text = "error";
            }
            ho_Image.Dispose();
            ho_ImageWord.Dispose();
            //ho_ImagePreprocessed.Dispose();
            //ho_ImagePreprocessedPadded.Dispose();

            hv_WindowHandle.Dispose();
            hv_DeepOcrResult.Dispose();
            hv_WindowHandlePreprocessed1.Dispose();
            hv_RecognitionImageWidth.Dispose();

        }



    }

}

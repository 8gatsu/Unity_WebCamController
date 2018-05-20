using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgProcGrayScale : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToGray(Color32[] colors, int width, int height) {
        Color32 rc = new Color32();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color32 c = colors[x + y * width];
                byte gray = (byte)(0.1f * c.r + 0.7f * c.g + 0.2f * c.b);
                //rc.r = rc.g = rc.b = gray;
                rc.r = rc.g = rc.b = gray;
                rc.a = 255;
                colors[x + y * width] = rc;
            }
        }

    }

    float[,] sobelX = new float[,] {
        { 1, 0, -1},
        { 2, 0, -2},
        { 1, 0, -1}
    };
    float[,] sobelY = new float[,] {
        {  1,  2,  1},
        {  0,  0,  0},
        { -1, -2, -1}
    };
    float[,] gaussian = new float[,] {
        { 1,1,1 },
        { 1,1,1 },
        { 1,1,1 }
    };


    public static void Filtering
        (
        byte[] src, byte[] dst, int width, int height,
        float[,] filter, int filter_numerator = 1, int filter_denominator = 1,
        bool absolute_value = true,
        bool normalize = false, float normalize_param = -1.0f
        )
    {
        int[] result = new int[dst.Length];
        for (int i = 0; i < result.Length; i++) { result[i] = 0; }

        int fsizeX = filter.GetLength(1);
        int fsizeY = filter.GetLength(0);
        int marginX = fsizeX / 2;
        int marginY = fsizeY / 2;

        for (int y = marginY; y < height - marginY - 1; y++)
        {
            for (int x = marginX; x < width - marginX - 1; x++)
            {
                for (int yi = 0; yi < fsizeY; yi++)
                {
                    for (int xi = 0; xi < fsizeX; xi++)
                    {
                        result[y * width + x] += (int)(filter[yi, xi] * src[(y + yi - marginY) * width + x + xi - marginX]);
                    }
                }
                if (filter_numerator != 1 || filter_denominator != 1) result[y * width + x] = result[y * width + x] * filter_numerator / filter_denominator;
                if (absolute_value && result[y * width + x] < 0) result[y * width + x] = -result[y * width + x];
            }
        }

        if (normalize)
        {
            int max = 1;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] > max) max = result[i];
            }
            if (normalize_param < 0)
            {
                int total = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    total += result[i];
                }
                int avg = total / (width * height);
                if (avg == 0) avg = 1;
                if (max >= avg * normalize_param) max = (int)(avg * normalize_param);
            }
            for (int i = 0; i < result.Length; i++) { dst[i] = (byte)(result[i] * 255 / max); }
        }
        else
        {
            for (int i = 0; i < result.Length; i++) { dst[i] = (byte)result[i]; }
        }

    }

    //int[,] sobelX = new int[,] {
    //    { 1, 0, -1},
    //    { 2, 0, -2},
    //    { 1, 0, -1}
    //};
    //int[,] sobelY = new int[,] {
    //    {  1,  2,  1},
    //    {  0,  0,  0},
    //    { -1, -2, -1}
    //};
    //int[,] gaussian = new int[,] {
    //    { 1,1,1 },
    //    { 1,1,1 },
    //    { 1,1,1 }
    //};


    //public static void Filtering
    //    (
    //    byte[] src, byte[] dst, int width, int height,
    //    int[,] filter, int filter_numerator = 1, int filter_denominator = 1,
    //    bool absolute_value = true,
    //    bool normalize = false, float normalize_param = -1.0f
    //    )
    //{
    //    int[] result = new int[dst.Length];
    //    for (int i = 0; i < result.Length; i++) { result[i] = 0; }

    //    int fsizeX = filter.GetLength(1);
    //    int fsizeY = filter.GetLength(0);
    //    int marginX = fsizeX / 2;
    //    int marginY = fsizeY / 2;

    //    int endpt = dst.Length;
    //    for (int yi = 0; yi < fsizeY; yi++)
    //    {
    //        for (int xi = 0; xi < fsizeX; xi++)
    //        {
    //            int fv = filter[yi, xi];
    //            for (int y = 0; y < height; y++)
    //            {
    //                if((y - (yi - marginY)) < height) {
    //                    int si = y * width;
    //                    int ri = (y - (yi - marginY)) * width - (xi - marginX);
    //                    for (int x = 0; x - (xi - marginX) < width; x++)
    //                    {
    //                        if (
    //                            0 <= x - (xi - marginX)
    //                            && si < endpt
    //                            && 0 <= ri
    //                            && ri < endpt
    //                            )
    //                        {
    //                            result[ri] += (int)(fv * src[si]);
    //                        }
    //                        si++;
    //                        ri++;
    //                    }

    //                }

    //            }
    //        }
    //    }
    //    if (filter_numerator != 1 || filter_denominator != 1 || absolute_value)
    //    {
    //        for(int i=0; i < result.Length; i++) {
    //            if (filter_numerator != 1 || filter_denominator != 1) result[i] = result[i] * filter_numerator / filter_denominator;
    //            if (absolute_value && result[i] < 0) result[i] = -result[i];
    //        }
    //    }




    //    if (normalize)
    //    {
    //        int max = 1;
    //        for (int i = 0; i < result.Length; i++)
    //        {
    //            if (result[i] > max) max = result[i];
    //        }
    //        if (normalize_param < 0)
    //        {
    //            int total = 0;
    //            for (int i = 0; i < result.Length; i++)
    //            {
    //                total += result[i];
    //            }
    //            int avg = total / (width * height);
    //            if (avg == 0) avg = 1;
    //            if (max >= avg * normalize_param) max = (int)(avg * normalize_param);
    //        }
    //        for (int i = 0; i < result.Length; i++) { dst[i] = (byte)(result[i] * 255 / max); }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < result.Length; i++) { dst[i] = (byte)result[i]; }
    //    }

    //}


    void ByteToColor(byte[] src, Color32[] dst) {
        int length = src.Length >= dst.Length ? dst.Length : src.Length;
        if (length == 0) return;
        for (int i = 0; i < length; i++) {
            dst[i].r = dst[i].g = dst[i].b = src[i];
        }
    }

    void ColorToByte(Color32[] src, byte[] dst)
    {
        int length = src.Length >= dst.Length ? dst.Length : src.Length;
        if (length == 0) return;
        for (int i = 0; i < length; i++)
        {
            dst[i]= src[i].r;
        }

    }

    public void Sample1(Color32[] colors, int width, int height) {
        byte[] tmp = new byte[colors.Length];

        ColorToByte(colors, tmp);
        Filtering(tmp, tmp, width, height, gaussian, 1, 9);
        Filtering(tmp, tmp, width, height, sobelX, 1, 1, true, true, 4.0f);
        ByteToColor(tmp, colors);
    }

    public int threshold = 128;
    public void Threshold(Color32[] colors, int width, int height)
    {
        Color32 rc = new Color32();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color32 c = colors[x + y * width];
                byte gray = (byte)( (c.r> threshold) ? 255:0);
                rc.r = rc.g = rc.b = gray;
                rc.a = 255;
                colors[x + y * width] = rc;
            }
        }
    }

}

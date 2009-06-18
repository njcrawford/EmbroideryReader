using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace embroideryThumbs
{
    //[Guid("69EA0D51-1BD4-4c6e-9AE0-B13C915A59E3")]
    //public interface EmbThumbnailInterface
    //{
    //    //COM visible function go here
    //}

    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    } 

    [ComImport, Guid("0000010c-0000-0000-c000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        void GetClassID(out Guid pClassID);
    }

    [ComImport, Guid("0000010b-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistFile : IPersist
    {
        //new void GetClassID(out Guid pClassID);
        [PreserveSig]
        int IsDirty();

        [PreserveSig]
        void Load([In, MarshalAs(UnmanagedType.LPWStr)]
            string pszFileName, uint dwMode);

        [PreserveSig]
        void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

        [PreserveSig]
        void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig]
        void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
    }

    [ComImportAttribute()]
    [GuidAttribute("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    interface IExtractImage
    {
        void GetLocation(
            [Out, MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder pszPathBuffer,
            int cch,
            ref int pdwPriority,
            ref SIZE prgSize,
            int dwRecClrDepth,
            ref int pdwFlags);

        void Extract(out IntPtr phBmpThumbnail);
    }

    //[Guid("xxxx"),
    //InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //public interface IExtractImage2 
    //{
    //    // from IExtractImage
    //    void Extract();
    //    void GetLocation();

    //    HRESULT GetDateStamp([Out] System.Runtime.InteropServices.ComTypes.FILETIME* pDateStamp);
    //}

    [Guid("EA7D5329-E9D7-49fb-9F55-A12D1A2ADB5D"),
        InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface EmbThumbnailEvents
    {
    }



    [Guid("7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E"),
        ClassInterface(ClassInterfaceType.None),
        ComSourceInterfaces(typeof(EmbThumbnailEvents))]
    public class EmbThumbnail : IPersistFile, IExtractImage
    {
        PesFile.PesFile designFile;

        public void GetClassID(out Guid pClassID)
        {
            // not implemented, but won't compile without this
            pClassID = new Guid("7E3EF3E8-39D4-4150-9EFF-58C71A1F4F9E");
        }

        public int IsDirty()
        {
            return 0;
        }

        public void Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode)
        {
            if(pszFileName.Substring(pszFileName.Length - 4).ToLower() == ".pes")
            {
                designFile = new PesFile.PesFile(pszFileName);
            }
        }

        public void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.Bool)] bool fRemember)
        {
            //not implemented
        }

        public void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName)
        {
            //not implemented
        }

        public void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName)
        {
            //not implemented
        }

        public void GetLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPathBuffer, int cch, ref int pdwPriority, ref SIZE prgSize, int dwRecClrDepth, ref int pdwFlags)
        {
            //not implemented
        }

        public unsafe void Extract(out IntPtr phBmpThumbnail)
        {
            System.Drawing.Bitmap designBitmap = designFile.designToBitmap(3, false, 0);

            IntPtr hBmp = designBitmap.GetHbitmap();

            phBmpThumbnail = new IntPtr();

            // Assuming you already have hBmp as the handle to your Bitmap object...
            if (IntPtr.Size == 4)
            {
                int* pBuffer = (int*)phBmpThumbnail.ToPointer();
                *pBuffer = hBmp.ToInt32();

                // IMO, casting back to (void*) is not necessary.
                // I guess just for formality, that pBuffer points
                // to an object, not an int.  :)
                phBmpThumbnail = new IntPtr((void*)pBuffer);
            }
            else // 8-bytes, or 64-bit
            {
                long* pBuffer = (long*)phBmpThumbnail.ToPointer();
                *pBuffer = hBmp.ToInt64();
                phBmpThumbnail = new IntPtr((void*)pBuffer);
            }
        }
    }
}

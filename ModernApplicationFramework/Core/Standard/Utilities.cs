﻿/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

/**************************************************************************\
    Copyright Microsoft Corporation. All Rights Reserved.
\**************************************************************************/

// This file contains general utilities to aid in development.
// Classes here generally shouldn't be exposed publicly since
// they're not particular to any library functionality.
// Because the classes here are internal, it's likely this file
// might be included in multiple assemblies.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.Core.Standard
{
    internal static class Utility
    {
        private static readonly Version OsVersion = Environment.OSVersion.Version;

        private static readonly Version PresentationFrameworkVersion =
            Assembly.GetAssembly(typeof(Window)).GetName().Version;

        // This can be cached.  It's not going to change under reasonable circumstances.
        private static int _sBitDepth; // = 0;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsOsVistaOrNewer => OsVersion >= new Version(6, 0);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsOsWindows7OrNewer => OsVersion >= new Version(6, 1);

        /// <summary>
        ///     Is this using WPF4?
        /// </summary>
        /// <remarks>
        ///     There are a few specific bugs in Window in 3.5SP1 and below that require workarounds
        ///     when handling WM_NCCALCSIZE on the HWND.
        /// </remarks>
        public static bool IsPresentationFrameworkVersionLessThan4 => PresentationFrameworkVersion < new Version(4, 0);

        public static void AddDependencyPropertyChangeListener(object component, DependencyProperty property,
                                                               EventHandler listener)
        {
            if (component == null)
            {
                return;
            }
            Assert.IsNotNull(property);
            Assert.IsNotNull(listener);

            var dpd = DependencyPropertyDescriptor.FromProperty(property, component.GetType());
            dpd.AddValueChanged(component, listener);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static bool AreStreamsEqual(Stream left, Stream right)
        {
            if (null == left)
            {
                return right == null;
            }
            if (null == right)
            {
                return false;
            }

            if (!left.CanRead || !right.CanRead)
            {
                throw new NotSupportedException("The streams can't be read for comparison");
            }

            if (left.Length != right.Length)
            {
                return false;
            }

            var length = (int) left.Length;

            // seek to beginning
            left.Position = 0;
            right.Position = 0;

            // total bytes read
            var totalReadLeft = 0;
            var totalReadRight = 0;

            // bytes read on this iteration
            var cbReadLeft = 0;
            var cbReadRight = 0;

            // where to store the read data
            var leftBuffer = new byte[512];
            var rightBuffer = new byte[512];

            // pin the left buffer
            var handleLeft = GCHandle.Alloc(leftBuffer, GCHandleType.Pinned);
            var ptrLeft = handleLeft.AddrOfPinnedObject();

            // pin the right buffer
            var handleRight = GCHandle.Alloc(rightBuffer, GCHandleType.Pinned);
            var ptrRight = handleRight.AddrOfPinnedObject();

            try
            {
                while (totalReadLeft < length)
                {
                    Assert.AreEqual(totalReadLeft, totalReadRight);

                    cbReadLeft = left.Read(leftBuffer, 0, leftBuffer.Length);
                    cbReadRight = right.Read(rightBuffer, 0, rightBuffer.Length);

                    // verify the contents are an exact match
                    if (cbReadLeft != cbReadRight)
                    {
                        return false;
                    }

                    if (!_MemCmp(ptrLeft, ptrRight, cbReadLeft))
                    {
                        return false;
                    }

                    totalReadLeft += cbReadLeft;
                    totalReadRight += cbReadRight;
                }

                Assert.AreEqual(cbReadLeft, cbReadRight);
                Assert.AreEqual(totalReadLeft, totalReadRight);
                Assert.AreEqual(length, totalReadLeft);

                return true;
            }
            finally
            {
                handleLeft.Free();
                handleRight.Free();
            }
        }

        /// <summary>Convert a native integer that represent a color with an alpha channel into a Color struct.</summary>
        /// <param name="color">The integer that represents the color.  Its bits are of the format 0xAARRGGBB.</param>
        /// <returns>A Color representation of the parameter.</returns>
        public static Color ColorFromArgbDword(uint color)
        {
            return Color.FromArgb(
                (byte) ((color & 0xFF000000) >> 24),
                (byte) ((color & 0x00FF0000) >> 16),
                (byte) ((color & 0x0000FF00) >> 8),
                (byte) ((color & 0x000000FF) >> 0));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void CopyStream(Stream destination, Stream source)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(destination);

            destination.Position = 0;

            // If we're copying from, say, a web stream, don't fail because of this.
            if (source.CanSeek)
            {
                source.Position = 0;

                // Consider that this could throw because 
                // the source stream doesn't know it's size...
                destination.SetLength(source.Length);
            }

            var buffer = new byte[4096];
            int cbRead;

            do
            {
                cbRead = source.Read(buffer, 0, buffer.Length);
                if (0 != cbRead)
                {
                    destination.Write(buffer, 0, cbRead);
                }
            } while (buffer.Length == cbRead);

            // Reset the Seek pointer before returning.
            destination.Position = 0;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
        }

        // Caller is responsible for destroying the HICON
        // Caller is responsible to ensure that GDI+ has been initialized.
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static IntPtr GenerateHicon(ImageSource image, Size dimensions)
        {
            if (image == null)
            {
                return IntPtr.Zero;
            }

            // If we're getting this from a ".ico" resource, then it comes through as a BitmapFrame.
            // We can use leverage this as a shortcut to get the right 16x16 representation
            // because DrawImage doesn't do that for us.
            BitmapFrame bf = image as BitmapFrame;
            if (bf != null)
            {
                if (bf.Decoder != null)
                    bf = GetBestMatch(bf.Decoder.Frames, (int) dimensions.Width, (int) dimensions.Height);
            }
            else
            {
                // Constrain the dimensions based on the aspect ratio.
                var drawingDimensions = new Rect(0, 0, dimensions.Width, dimensions.Height);

                // There's no reason to assume that the requested image dimensions are square.
                var renderRatio = dimensions.Width/dimensions.Height;
                var aspectRatio = image.Width/image.Height;

                // If it's smaller than the requested size, then place it in the middle and pad the image.
                if (image.Width <= dimensions.Width && image.Height <= dimensions.Height)
                {
                    drawingDimensions = new Rect((dimensions.Width - image.Width)/2,
                        (dimensions.Height - image.Height)/2, image.Width, image.Height);
                }
                else
                    if (renderRatio > aspectRatio)
                    {
                        var scaledRenderWidth = image.Width/image.Height*dimensions.Width;
                        drawingDimensions = new Rect((dimensions.Width - scaledRenderWidth)/2, 0, scaledRenderWidth,
                            dimensions.Height);
                    }
                    else
                        if (renderRatio < aspectRatio)
                        {
                            var scaledRenderHeight = image.Height/image.Width*dimensions.Height;
                            drawingDimensions = new Rect(0, (dimensions.Height - scaledRenderHeight)/2, dimensions.Width,
                                scaledRenderHeight);
                        }

                var dv = new DrawingVisual();
                var dc = dv.RenderOpen();
                dc.DrawImage(image, drawingDimensions);
                dc.Close();

                var bmp = new RenderTargetBitmap((int) dimensions.Width, (int) dimensions.Height, 96, 96,
                    PixelFormats.Pbgra32);
                bmp.Render(dv);
                bf = BitmapFrame.Create(bmp);
            }

            // Using GDI+ to convert to an HICON.
            // I'd rather not duplicate their code.
            using (var memstm = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(bf);
                enc.Save(memstm);

                using (var istm = new ManagedIStream(memstm))
                {
                    // We are not bubbling out GDI+ errors when creating the native image fails.
                    var bitmap = IntPtr.Zero;
                    try
                    {
                        var gpStatus = NativeMethods.GdipCreateBitmapFromStream(istm, out bitmap);
                        if (Status.Ok != gpStatus)
                        {
                            return IntPtr.Zero;
                        }

                        IntPtr hicon;
                        gpStatus = NativeMethods.GdipCreateHICONFromBitmap(bitmap, out hicon);
                        if (Status.Ok != gpStatus)
                        {
                            return IntPtr.Zero;
                        }

                        // Caller is responsible for freeing this.
                        return hicon;
                    }
                    finally
                    {
                        SafeDisposeImage(ref bitmap);
                    }
                }
            }
        }

        /// <summary>
        ///     Utility to help classes catenate their properties for implementing ToString().
        /// </summary>
        /// <param name="source">The StringBuilder to catenate the results into.</param>
        /// <param name="propertyName">The name of the property to be catenated.</param>
        /// <param name="value">The value of the property to be catenated.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void GeneratePropertyString(StringBuilder source, string propertyName, string value)
        {
            Assert.IsNotNull(source);
            Assert.IsFalse(string.IsNullOrEmpty(propertyName));

            if (0 != source.Length)
            {
                source.Append(' ');
            }

            source.Append(propertyName);
            source.Append(": ");
            if (string.IsNullOrEmpty(value))
            {
                source.Append("<null>");
            }
            else
            {
                source.Append('\"');
                source.Append(value);
                source.Append('\"');
            }
        }

        /// <summary>
        ///     Generates ToString functionality for a struct.  This is an expensive way to do it,
        ///     it exists for the sake of debugging while classes are in flux.
        ///     Eventually this should just be removed and the classes should
        ///     do this without reflection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Obsolete]
        public static string GenerateToString<T>(T @object) where T : struct
        {
            var sbRet = new StringBuilder();
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (0 != sbRet.Length)
                {
                    sbRet.Append(", ");
                }
                Assert.AreEqual(0, property.GetIndexParameters().Length);
                var value = property.GetValue(@object, null);
                var format = null == value ? "{0}: <null>" : "{0}: \"{1}\"";
                sbRet.AppendFormat(format, property.Name, value);
            }
            return sbRet.ToString();
        }


        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int GET_X_LPARAM(IntPtr lParam)
        {
            return Loword(lParam.ToInt32());
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int GET_Y_LPARAM(IntPtr lParam)
        {
            return Hiword(lParam.ToInt32());
        }

        public static BitmapFrame GetBestMatch(IList<BitmapFrame> frames, int width, int height)
        {
            return _GetBestMatch(frames, _GetBitDepth(), width, height);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool GuidTryParse(string guidString, out Guid guid)
        {
            Verify.IsNeitherNullNorEmpty(guidString, "guidString");

            try
            {
                guid = new Guid(guidString);
                return true;
            }
            catch (FormatException) {}
            catch (OverflowException) {}
            // Doesn't seem to be a valid guid.
            guid = default(Guid);
            return false;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string HashStreamMd5(Stream stm)
        {
            stm.Position = 0;
            var hashBuilder = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                foreach (var b in md5.ComputeHash(stm))
                {
                    hashBuilder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                }
            }

            return hashBuilder.ToString();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int Hiword(int i)
        {
            return (short) (i >> 16);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(int value, int mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(uint value, uint mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(long value, long mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool IsFlagSet(ulong value, ulong mask)
        {
            return 0 != (value & mask);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int Loword(int i)
        {
            return (short) (i & 0xFFFF);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static bool MemCmp(byte[] left, byte[] right, int cb)
        {
            Assert.IsNotNull(left);
            Assert.IsNotNull(right);

            Assert.IsTrue(cb <= Math.Min(left.Length, right.Length));

            // pin this buffer
            var handleLeft = GCHandle.Alloc(left, GCHandleType.Pinned);
            var ptrLeft = handleLeft.AddrOfPinnedObject();

            // pin the other buffer
            var handleRight = GCHandle.Alloc(right, GCHandleType.Pinned);
            var ptrRight = handleRight.AddrOfPinnedObject();

            var fRet = _MemCmp(ptrLeft, ptrRight, cb);

            handleLeft.Free();
            handleRight.Free();

            return fRet;
        }

        public static void RemoveDependencyPropertyChangeListener(object component, DependencyProperty property,
                                                                  EventHandler listener)
        {
            if (component == null)
            {
                return;
            }
            Assert.IsNotNull(property);
            Assert.IsNotNull(listener);

            var dpd = DependencyPropertyDescriptor.FromProperty(property, component.GetType());
            dpd.RemoveValueChanged(component, listener);
        }

        /// <summary>The native RGB macro.</summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static int Rgb(Color c)
        {
            return c.R | (c.G << 8) | (c.B << 16);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SafeCoTaskMemFree(ref IntPtr ptr)
        {
            var p = ptr;
            ptr = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                Marshal.FreeCoTaskMem(p);
            }
        }

        /// <summary>
        ///     Simple guard against the exceptions that File.Delete throws on null and empty strings.
        /// </summary>
        /// <param name="path">The path to delete.  Unlike File.Delete, this can be null or empty.</param>
        /// <remarks>
        ///     Note that File.Delete, and by extension SafeDeleteFile, does not throw an exception
        ///     if the file does not exist.
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>GDI's DeleteObject</summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDeleteObject(ref IntPtr gdiObject)
        {
            var p = gdiObject;
            gdiObject = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                NativeMethods.DeleteObject(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDestroyIcon(ref IntPtr hicon)
        {
            var p = hicon;
            hicon = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                NativeMethods.DestroyIcon(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDestroyWindow(ref IntPtr hwnd)
        {
            var p = hwnd;
            hwnd = IntPtr.Zero;
            if (NativeMethods.IsWindow(p))
            {
                NativeMethods.DestroyWindow(p);
            }
        }


        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDispose<T>(ref T disposable) where T : IDisposable
        {
            // Dispose can safely be called on an object multiple times.
            IDisposable t = disposable;
            disposable = default(T);
            t?.Dispose();
        }

        /// <summary>GDI+'s DisposeImage</summary>
        /// <param name="gdipImage"></param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static void SafeDisposeImage(ref IntPtr gdipImage)
        {
            var p = gdipImage;
            gdipImage = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                NativeMethods.GdipDisposeImage(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SafeFreeHGlobal(ref IntPtr hglobal)
        {
            var p = hglobal;
            hglobal = IntPtr.Zero;
            if (IntPtr.Zero != p)
            {
                Marshal.FreeHGlobal(p);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SafeRelease<T>(ref T comObject) where T : class
        {
            var t = comObject;
            comObject = default(T);
            if (null != t)
            {
                Assert.IsTrue(Marshal.IsComObject(t));
                Marshal.ReleaseComObject(t);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string UrlDecode(string url)
        {
            if (url == null)
            {
                return null;
            }

            var decoder = new UrlDecoder(url.Length, Encoding.UTF8);
            var length = url.Length;
            for (var i = 0; i < length; ++i)
            {
                var ch = url[i];

                if (ch == '+')
                {
                    decoder.AddByte((byte) ' ');
                    continue;
                }

                if (ch == '%' && i < length - 2)
                {
                    // decode %uXXXX into a Unicode character.
                    if (url[i + 1] == 'u' && i < length - 5)
                    {
                        var a = _HexToInt(url[i + 2]);
                        var b = _HexToInt(url[i + 3]);
                        var c = _HexToInt(url[i + 4]);
                        var d = _HexToInt(url[i + 5]);
                        if (a >= 0 && b >= 0 && c >= 0 && d >= 0)
                        {
                            decoder.AddChar((char) ((a << 12) | (b << 8) | (c << 4) | d));
                            i += 5;

                            continue;
                        }
                    }
                    else
                    {
                        // decode %XX into a Unicode character.
                        var a = _HexToInt(url[i + 1]);
                        var b = _HexToInt(url[i + 2]);

                        if (a >= 0 && b >= 0)
                        {
                            decoder.AddByte((byte) ((a << 4) | b));
                            i += 2;

                            continue;
                        }
                    }
                }

                // Add any 7bit character as a byte.
                if ((ch & 0xFF80) == 0)
                {
                    decoder.AddByte((byte) ch);
                }
                else
                {
                    decoder.AddChar(ch);
                }
            }

            return decoder.GetString();
        }

        /// <summary>
        ///     Encodes a URL string.  Duplicated functionality from System.Web.HttpUtility.UrlEncode.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Duplicated from System.Web.HttpUtility because System.Web isn't part of the client profile.
        ///     URL Encoding replaces ' ' with '+' and unsafe ASCII characters with '%XX'.
        ///     Safe characters are defined in RFC2396 (http://www.ietf.org/rfc/rfc2396.txt).
        ///     They are the 7-bit ASCII alphanumerics and the mark characters "-_.!~*'()".
        ///     This implementation does not treat '~' as a safe character to be consistent with the System.Web version.
        /// </remarks>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static string UrlEncode(string url)
        {
            if (url == null)
            {
                return null;
            }

            var bytes = Encoding.UTF8.GetBytes(url);

            var needsEncoding = false;
            var unsafeCharCount = 0;
            foreach (var b in bytes)
            {
                if (b == ' ')
                {
                    needsEncoding = true;
                }
                else
                    if (!_UrlEncodeIsSafe(b))
                    {
                        ++unsafeCharCount;
                        needsEncoding = true;
                    }
            }

            if (needsEncoding)
            {
                var buffer = new byte[bytes.Length + unsafeCharCount*2];
                var writeIndex = 0;
                foreach (var b in bytes)
                {
                    if (_UrlEncodeIsSafe(b))
                    {
                        buffer[writeIndex++] = b;
                    }
                    else
                        if (b == ' ')
                        {
                            buffer[writeIndex++] = (byte) '+';
                        }
                        else
                        {
                            buffer[writeIndex++] = (byte) '%';
                            buffer[writeIndex++] = _IntToHex((b >> 4) & 0xF);
                            buffer[writeIndex++] = _IntToHex(b & 0xF);
                        }
                }
                bytes = buffer;
                Assert.AreEqual(buffer.Length, writeIndex);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        /// From a list of BitmapFrames find the one that best matches the requested dimensions.
        /// The methods used here are copied from Win32 sources.  We want to be consistent with
        /// system behaviors.
        private static BitmapFrame _GetBestMatch(IList<BitmapFrame> frames, int bitDepth, int width, int height)
        {
            var bestScore = int.MaxValue;
            var bestBpp = 0;
            var bestIndex = 0;

            var isBitmapIconDecoder = frames[0].Decoder is IconBitmapDecoder;

            for (var i = 0; i < frames.Count && bestScore != 0; ++i)
            {
                var currentIconBitDepth = isBitmapIconDecoder
                    ? frames[i].Thumbnail.Format.BitsPerPixel
                    : frames[i].Format.BitsPerPixel;

                if (currentIconBitDepth == 0)
                {
                    currentIconBitDepth = 8;
                }

                var score = _MatchImage(frames[i], bitDepth, width, height, currentIconBitDepth);
                if (score < bestScore)
                {
                    bestIndex = i;
                    bestBpp = currentIconBitDepth;
                    bestScore = score;
                }
                else
                    if (score == bestScore)
                    {
                        // Tie breaker: choose the higher color depth.  If that fails, choose first one.
                        if (bestBpp < currentIconBitDepth)
                        {
                            bestIndex = i;
                            bestBpp = currentIconBitDepth;
                        }
                    }
            }

            return frames[bestIndex];
        }

        private static int _GetBitDepth()
        {
            if (_sBitDepth == 0)
            {
                using (var dc = SafeDC.GetDesktop())
                {
                    _sBitDepth = NativeMethods.GetDeviceCaps(dc, DeviceCap.BITSPIXEL)
                                 *NativeMethods.GetDeviceCaps(dc, DeviceCap.PLANES);
                }
            }
            return _sBitDepth;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static int _HexToInt(char h)
        {
            if (h >= '0' && h <= '9')
            {
                return h - '0';
            }

            if (h >= 'a' && h <= 'f')
            {
                return h - 'a' + 10;
            }

            if (h >= 'A' && h <= 'F')
            {
                return h - 'A' + 10;
            }

            Assert.Fail("Invalid hex character " + h);
            return -1;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static byte _IntToHex(int n)
        {
            Assert.BoundedInteger(0, n, 16);
            if (n <= 9)
            {
                return (byte) (n + '0');
            }
            return (byte) (n - 10 + 'A');
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _IsAsciiAlphaNumeric(byte b)
        {
            return (b >= 'a' && b <= 'z')
                   || (b >= 'A' && b <= 'Z')
                   || (b >= '0' && b <= '9');
        }

        private static int _MatchImage(BitmapFrame frame, int bitDepth, int width, int height, int bpp)
        {
            var score = 2*_WeightedAbs(bpp, bitDepth, false) +
                        _WeightedAbs(frame.PixelWidth, width, true) +
                        _WeightedAbs(frame.PixelHeight, height, true);

            return score;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _MemCmp(IntPtr left, IntPtr right, long cb)
        {
            var offset = 0;

            for (; offset < cb - sizeof(long); offset += sizeof(long))
            {
                var left64 = Marshal.ReadInt64(left, offset);
                var right64 = Marshal.ReadInt64(right, offset);

                if (left64 != right64)
                {
                    return false;
                }
            }

            for (; offset < cb; offset += sizeof(byte))
            {
                var left8 = Marshal.ReadByte(left, offset);
                var right8 = Marshal.ReadByte(right, offset);

                if (left8 != right8)
                {
                    return false;
                }
            }

            return true;
        }

        // HttpUtility's UrlEncode is slightly different from the RFC.
        // RFC2396 describes unreserved characters as alphanumeric or
        // the list "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
        // The System.Web version unnecessarily escapes '~', which should be okay...
        // Keeping that same pattern here just to be consistent.
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private static bool _UrlEncodeIsSafe(byte b)
        {
            if (_IsAsciiAlphaNumeric(b))
            {
                return true;
            }

            switch ((char) b)
            {
                case '-':
                case '_':
                case '.':
                case '!':
                //case '~':
                case '*':
                case '\'':
                case '(':
                case ')':
                    return true;
            }

            return false;
        }

        private static int _WeightedAbs(int valueHave, int valueWant, bool fPunish)
        {
            var diff = valueHave - valueWant;

            if (diff < 0)
            {
                diff = (fPunish ? -2 : -1)*diff;
            }

            return diff;
        }

        private class UrlDecoder
        {
            private readonly byte[] _byteBuffer;
            private readonly char[] _charBuffer;
            private readonly Encoding _encoding;
            private int _byteCount;
            private int _charCount;

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public UrlDecoder(int size, Encoding encoding)
            {
                _encoding = encoding;
                _charBuffer = new char[size];
                _byteBuffer = new byte[size];
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void AddByte(byte b)
            {
                _byteBuffer[_byteCount++] = b;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public void AddChar(char ch)
            {
                _FlushBytes();
                _charBuffer[_charCount++] = ch;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string GetString()
            {
                _FlushBytes();
                if (_charCount > 0)
                {
                    return new string(_charBuffer, 0, _charCount);
                }
                return "";
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            private void _FlushBytes()
            {
                if (_byteCount > 0)
                {
                    _charCount += _encoding.GetChars(_byteBuffer, 0, _byteCount, _charBuffer, _charCount);
                    _byteCount = 0;
                }
            }
        }

        #region Extension Methods

        public static bool IsThicknessNonNegative(Thickness thickness)
        {
            if (!IsDoubleFiniteAndNonNegative(thickness.Top))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Left))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Bottom))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(thickness.Right))
            {
                return false;
            }

            return true;
        }

        public static bool IsCornerRadiusValid(CornerRadius cornerRadius)
        {
            if (!IsDoubleFiniteAndNonNegative(cornerRadius.TopLeft))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(cornerRadius.TopRight))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(cornerRadius.BottomLeft))
            {
                return false;
            }

            if (!IsDoubleFiniteAndNonNegative(cornerRadius.BottomRight))
            {
                return false;
            }

            return true;
        }

        public static bool IsDoubleFiniteAndNonNegative(double d)
        {
            if (double.IsNaN(d) || double.IsInfinity(d) || d < 0)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
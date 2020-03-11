﻿using System;
using System.Collections.Generic;
using OpenCvSharp.Util;

namespace OpenCvSharp
{
    /// <summary>
    /// Base class for tonemapping algorithms - tools that are used to map HDR image to 8-bit range.
    /// </summary>
    public class Tonemap : Algorithm
    {
        private Tonemap.Ptr? ptrObj;

        /// <summary>
        /// Creates instance by Tonemap.Create
        /// </summary>
        protected Tonemap(IntPtr p)
        {
            ptrObj = new Tonemap.Ptr(p);
            ptr = ptrObj.Get();
        }

        /// <summary>
        /// Creates simple linear mapper with gamma correction
        /// </summary>
        /// <param name="gamma">positive value for gamma correction.
        /// Gamma value of 1.0 implies no correction, gamma equal to 2.2f is suitable for most displays.
        /// Generally gamma &gt; 1 brightens the image and gamma &lt; 1 darkens it.</param>
        /// <returns></returns>
        public static Tonemap Create(float gamma = 1f)
        {
            NativeMethods.HandleException(
                NativeMethods.photo_createTonemap(gamma, out var ptr));
            return new Tonemap(ptr);
        }

        /// <summary>
        /// Releases managed resources
        /// </summary>
        protected override void DisposeManaged()
        {
            ptrObj?.Dispose();
            ptrObj = null;
            base.DisposeManaged();
        }

        /// <summary>
        /// Tonemaps image
        /// </summary>
        /// <param name="src">source image - CV_32FC3 Mat (float 32 bits 3 channels)</param>
        /// <param name="dst">destination image - CV_32FC3 Mat with values in [0, 1] range</param>
        public virtual void Process(InputArray src, OutputArray dst)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));

            src.ThrowIfDisposed();
            dst.ThrowIfNotReady();

            NativeMethods.HandleException(
                NativeMethods.photo_Tonemap_process(ptr, src.CvPtr, dst.CvPtr));

            GC.KeepAlive(this);
            GC.KeepAlive(src);
            dst.Fix();
        }

        /// <summary>
        /// Gets or sets positive value for gamma correction. Gamma value of 1.0 implies no correction, gamma
        /// equal to 2.2f is suitable for most displays.
        /// Generally gamma &gt; 1 brightens the image and gamma &lt; 1 darkens it.
        /// </summary>
        public float Gamma
        {
            get
            {
                ThrowIfDisposed();
                NativeMethods.HandleException(
                    NativeMethods.photo_Tonemap_getGamma(ptr, out var ret));
                GC.KeepAlive(this);
                return ret;
            }
            set
            {
                ThrowIfDisposed();
                NativeMethods.HandleException(
                    NativeMethods.photo_Tonemap_setGamma(ptr, value));
                GC.KeepAlive(this);
            }
        }

        private class Ptr : OpenCvSharp.Ptr
        {
            public Ptr(IntPtr ptr) : base(ptr)
            {
            }

            public override IntPtr Get()
            {
                NativeMethods.HandleException(
                    NativeMethods.photo_Ptr_Tonemap_get(ptr, out var ret));
                GC.KeepAlive(this);
                return ret;
            }

            protected override void DisposeUnmanaged()
            {
                NativeMethods.HandleException(
                    NativeMethods.photo_Ptr_Tonemap_delete(ptr));
                base.DisposeUnmanaged();
            }
        }
    }
}

// #region License
// // /*
// // The MIT License (MIT)
// //
// // Copyright (c) 2013-2014 The SharpFlame Authors.
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in
// // all copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.
// // */
// #endregion
//
using System;
using Gtk;
using Eto.Platform.GtkSharp;
using SharpFlame.Gui;
using SharpFlame.Gui.Controls;

namespace SharpFlame.Gui.Gtk
{
    internal class LinuxGLSurfaceHandler : GtkControl<GLDrawingArea, GLSurface>, IGLSurface
    {
        public override GLDrawingArea CreateControl()
        {
            Control = new GLDrawingArea();
            Control.Initialized += new System.EventHandler(onInitalized);
            Control.Paint += new System.EventHandler(onPaint);
            Control.ShuttingDown += new System.EventHandler(onShuttingDown);

            return Control;
        }

        public event EventHandler Initialized = delegate {}; 
        private void onInitalized(object obj, EventArgs e) 
        {
            Initialized (obj, e);
        }

        public event EventHandler Paint = delegate {}; 
        private void onPaint(object obj, EventArgs e) 
        {
            Paint (obj, e);
        }

        public event EventHandler ShuttingDown = delegate {};
        private void onShuttingDown(object obj, EventArgs e) 
        {
            ShuttingDown (obj, e);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public void MakeCurrent() 
        {
            Control.MakeCurrent ();
        }

        public void SwapBuffers()
        {
            Control.SwapBuffers ();
        }
    }
}
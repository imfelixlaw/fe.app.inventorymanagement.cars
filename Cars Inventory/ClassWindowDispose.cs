/*
 * Here is to do the disposing coding for window
 * in-order to do resource disposing and releasing
 */

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Cars_Inventory
{
    public partial class MainWindow : IDisposable
    {
        //-- IDisposable -- Start -->
        //-- All other IDisposable code on other source code will skip the comment, please refer to here
        private IntPtr handle; // Pointer to an external unmanaged resource.
        private Component component = new Component(); // Other managed resource this class uses.
        private bool disposed = false; // Track whether Dispose has been called.

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to take this object off the finalization queue
            // and prevent finalization code for this object from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly or indirectly by a user's code.
        // Managed and unmanaged resources can be disposed.
        // If disposing equals false, the method has been called by the runtime from inside the finalizer
        // and you should not reference other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed) // Check to see if Dispose has already been called.
            {
                if (disposing) // If disposing equals true, dispose all managed and unmanaged resources.
                {
                    component.Dispose(); // Dispose managed resources.
                }
                // Call the appropriate methods to clean up unmanaged resources here.
                // If disposing is false, only the following code is executed.
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true; // Note disposing has been done.
            }
        }

        // Use interop to call the method necessary to clean up the unmanaged resource.
        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~MainWindow() //--> Change to Object Name
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of readability and maintainability.
            Dispose(false);
        }
        //-- IDisposable -- End -->
    }

    public partial class WindowItemListByItem : IDisposable
    {
        private IntPtr handle; // Pointer to an external unmanaged resource.
        private Component component = new Component(); // Other managed resource this class uses.
        private bool disposed = false; // Track whether Dispose has been called.

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowItemListByItem()
        {
            Dispose(false);
        }
    }

    public partial class WindowProductGroup : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowProductGroup()
        {
            Dispose(false);
        }
    }

    public partial class WindowDisplayMessage : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowDisplayMessage()
        {
            Dispose(false);
        }
    }

    public partial class WindowProductGroup_GroupRelated : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowProductGroup_GroupRelated()
        {
            Dispose(false);
        }
    }

    public partial class WindowCentreStockIn : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowCentreStockIn()
        {
            Dispose(false);
        }
    }

    public partial class WindowItemDataDetails : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowItemDataDetails()
        {
            Dispose(false);
        }
    }

    public partial class WindowRptStockTake : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowRptStockTake()
        {
            Dispose(false);
        }
    }

    public partial class WindowRptStockOrder : IDisposable
    {
        private IntPtr handle;
        private Component component = new Component();
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing) { component.Dispose(); }
                CloseHandle(handle);
                handle = IntPtr.Zero;
                disposed = true;
            }
        }

        [DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);

        ~WindowRptStockOrder()
        {
            Dispose(false);
        }
    }
}
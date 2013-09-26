using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SPA
{
    public partial class richEx : RichTextBox
    {
        private readonly object mustHideCaretLocker = new object();

        private bool mustHideCaret;

        [DefaultValue(false)]
        public bool MustHideCaret
        {
            get
            {
                lock (this.mustHideCaretLocker)
                    return this.mustHideCaret;
            }
            set
            {
                TabStop = false;
                if (value)
                    SetHideCaret();
                else
                    SetShowCaret();
            }
        }

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);
        [DllImport("user32.dll", EntryPoint = "ShowCaret")]
        private static extern long ShowCaret(IntPtr hwnd);

        public richEx()
        {
            //VScroll += HandleRichTextBoxAdjustScroll;
            //TextChanged += HandleRichTextBoxAdjustScroll;
        }

        private void SetHideCaret()
        {
            //VScroll += richEx_VScroll;
            MouseDown += new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
            MouseUp += new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
            Resize += new EventHandler(ReadOnlyRichTextBox_Resize);
            HideCaret(Handle);
            lock (this.mustHideCaretLocker)
                this.mustHideCaret = true;
        }

        //void richEx_VScroll(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void SetShowCaret()
        {
            try
            {
                MouseDown -= new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
                MouseUp -= new MouseEventHandler(ReadOnlyRichTextBox_Mouse);
                Resize -= new EventHandler(ReadOnlyRichTextBox_Resize);
            }
            catch
            {
            }
            ShowCaret(Handle);
            lock (this.mustHideCaretLocker)
                this.mustHideCaret = false;
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (MustHideCaret)
            {
                HideCaret(Handle);
                this.Parent.Focus();//here we select parent control in my case it is panel
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            if (MustHideCaret)
                HideCaret(Handle);
        }

        private void ReadOnlyRichTextBox_Mouse(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HideCaret(Handle);
        }

        private void ReadOnlyRichTextBox_Resize(object sender, System.EventArgs e)
        {
            HideCaret(Handle);
        }


        //private const UInt32 SB_TOP = 0x6;
        //private const UInt32 WM_VSCROLL = 0x115;

        //[return: MarshalAs(UnmanagedType.Bool)]
        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg,
        //    IntPtr wParam, IntPtr lParam);

        //private void HandleRichTextBoxAdjustScroll(Object sender,
        //    EventArgs e)
        //{
        //    PostMessage(Handle, WM_VSCROLL, (IntPtr)SB_TOP, IntPtr.Zero);
        //}


    }
}
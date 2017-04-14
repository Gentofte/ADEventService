using System;
using System.Drawing;

namespace ADxSimpleAdapterWin.Forms
{
    // ================================================================================
    public partial class SafeRTF : System.Windows.Forms.RichTextBox
    {
        //const string _crlf = "\r\n";
        readonly string _crlf = Environment.NewLine;

        delegate void AppendTextCallback(string text);
        delegate void ClearCallback();
        delegate string GetTextCallback();
        delegate void SetTextCallback(string text);

        // http://www.worqx.com/color/color_wheel.htm

        // -----------------------------------------------------------------------------
        public SafeRTF()
        {
            InitializeComponent();
        }

        #region Properties
        // -----------------------------------------------------------------------------
        public new string Text
        {
            get { return GetText(); }
            set { SetText(value); }
        }

        // -----------------------------------------------------------------------------
        public int ClearDisplayOnLineCountThreshold
        { get; set; }

        // -----------------------------------------------------------------------------
        public bool ZeroBasedLineNumbers
        { get; set; }

        // -----------------------------------------------------------------------------
        public bool TimePrefixEnabled
        { get; set; }

        // -----------------------------------------------------------------------------
        public string NewLine
        {
            get { return _crlf; }
        }
        #endregion

        #region Public methods
        // -----------------------------------------------------------------------------
        public new void AppendText(string text)
        {
            // This helper method will prevent thread related traps when writing to RTB.

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.InvokeRequired)
            {
                //AppendTextCallback d = new AppendTextCallback(SafeAppendText);
                AppendTextCallback d = new AppendTextCallback(AppendText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (TimePrefixEnabled)
                {
                    text = DateTime.Now.ToString("HH:mm:ss.fff") + ": " + text;
                }

                // Ensure trailing linefeed
                if (!(text.EndsWith(_crlf)))
                {
                    text += _crlf;
                }

                int curLineCnt = Lines.Length;
                int resetLineCnt = ClearDisplayOnLineCountThreshold;

                // Errors is displayed in RED
                this.SelectionColor = Color.Black;
                if (text.IndexOf("E R R O R") > -1 || text.IndexOf("ERROR") > -1)
                {
                    this.SelectionColor = Color.Red;
                }

                if (resetLineCnt > 0)
                {
                    if (curLineCnt > resetLineCnt)
                    { base.Clear(); }

                    if (Lines.Length == 0)
                    { curLineCnt = 1; }

                    if (ZeroBasedLineNumbers) curLineCnt--;

                    base.AppendText(curLineCnt.ToString("0000") + ": " + text);
                }
                else
                {
                    base.AppendText(text);
                }
            }
        }

        // -----------------------------------------------------------------------------
        public new void Clear()
        {
            if (this.InvokeRequired)
            {
                ClearCallback d = new ClearCallback(Clear);
                this.Invoke(d, new object[] { });
            }
            else
            {
                base.Clear();
            }
        }
        #endregion

        #region Private methods
        // -----------------------------------------------------------------------------
        string GetText()
        {
            if (this.InvokeRequired)
            {
                GetTextCallback d = new GetTextCallback(GetText);
                return (string)this.Invoke(d, new object[] { });
            }
            else
            {
                return base.Text;
            }
        }

        // -----------------------------------------------------------------------------
        void SetText(string text)
        {
            if (this.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                base.Text = text;
            }
        }
        #endregion
    }
}

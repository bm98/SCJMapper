using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Joystick
{
  /// <summary>
  /// Implements a JS panel with some public methods to access the labels
  /// </summary>
  public partial class UC_JoyPanel : UserControl
  {
    public UC_JoyPanel( )
    {
      InitializeComponent( );
    }

    #region Strings

    public String Caption
    {
      set { gBox.Text = value; }
    }

    public String X
    {
      set { lbl1X.Text = value; }
    }

    public String Y
    {
      set { lbl1Y.Text = value; }
    }

    public String Z
    {
      set { lbl1Z.Text = value; }
    }

    public String Xr
    {
      set { lbl1Xr.Text = value; }
    }

    public String Yr
    {
      set { lbl1Yr.Text = value; }
    }

    public String Zr
    {
      set { lbl1Zr.Text = value; }
    }

    public String S1
    {
      set { lbl1S0.Text = value; }
    }

    public String S2
    {
      set { lbl1S1.Text = value; }
    }

    public String H1
    {
      set { lbl1Hat0.Text = value; }
    }

    public String H2
    {
      set { lbl1Hat1.Text = value; }
    }

    public String H3
    {
      set { lbl1Hat2.Text = value; }
    }

    public String H4
    {
      set { lbl1Hat3.Text = value; }
    }

    public String Button
    {
      set { lbl1Buttons.Text = value; }
    }

    #endregion


    #region Enables

    public Boolean Xe
    {
      set { lbl1X.Enabled = value; lX.Enabled = value; }
    }

    public Boolean Ye
    {
      set { lbl1Y.Enabled = value; lY.Enabled = value; }
    }

    public Boolean Ze
    {
      set { lbl1Z.Enabled = value; lZ.Enabled = value; }
    }

    public Boolean Xre
    {
      set { lbl1Xr.Enabled = value; lXr.Enabled = value; }
    }

    public Boolean Yre
    {
      set { lbl1Yr.Enabled = value; lYr.Enabled = value; }
    }

    public Boolean Zre
    {
      set { lbl1Zr.Enabled = value; lZr.Enabled = value; }
    }

    public Boolean S1e
    {
      set { lbl1S0.Enabled = value; lS0.Enabled = value; }
    }

    public Boolean S2e
    {
      set { lbl1S1.Enabled = value; lS1.Enabled = value; }
    }

    public Boolean H1e
    {
      set { lbl1Hat0.Enabled = value; lH0.Enabled = value; }
    }

    public Boolean H2e
    {
      set { lbl1Hat1.Enabled = value; lH1.Enabled = value; }
    }

    public Boolean H3e
    {
      set { lbl1Hat2.Enabled = value; lH2.Enabled = value; }
    }

    public Boolean H4e
    {
      set { lbl1Hat3.Enabled = value; lH3.Enabled = value; }
    }

    public Boolean Buttone
    {
      set { lbl1Buttons.Enabled = value; lB.Enabled = value; }
    }

    #endregion


  }
}

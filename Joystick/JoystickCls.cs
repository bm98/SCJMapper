using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System.Windows.Forms;

namespace Joystick
{
  /// <summary>
  /// Handles one JS device as DXInput device
  /// In addition provide some static tools to handle JS props here in one place
  /// Also owns the GUI i.e. the user control that shows all values
  /// </summary>
  class JoystickCls
  {
    private   Device m_device;

    private   JoystickState m_state = new JoystickState( );
    private   JoystickState m_prevState = new JoystickState( );

    private   Control m_hwnd;
    private int m_numPOVs = 0;
    private int m_sliderCount = 0;
    private String m_lastItem = "";

    private UC_JoyPanel m_jPanel = null; // the GUI panel


    /// <summary>
    /// Returns a CryEngine compatible hat direction
    /// </summary>
    /// <param name="value">The Hat value</param>
    /// <returns>The direction string</returns>
    private String HatDir( int value )
    {
      // Hats have a 360deg -> 36000 value reporting
      if ( value == 0 ) return "up";
      if ( value == 9000 ) return "right";
      if ( value == 18000 ) return "down";
      if ( value == 27000 ) return "left";
      return "";
    }

    /// <summary>
    /// Returns properly formatted jsn_ string
    /// </summary>
    /// <param name="jsNum">The JS number</param>
    /// <returns>The formatted JS name for the CryEngine XML</returns>
    static public String JSTag( int jsNum )
    {
      return "js" + jsNum.ToString( ) + "_";
    }


    /// <summary>
    /// Extract the JS number from a JS string
    /// </summary>
    /// <param name="jsTag">The JS string</param>
    /// <returns>The JS number</returns>
    static public int JSNum( String jsTag )
    {
      int retNum=0;
      if ( !String.IsNullOrEmpty( jsTag ) ) {
        int.TryParse( jsTag.Substring( 2, 1 ), out retNum );
      }
      return retNum;
    }

    /// <summary>
    /// The povides the JS ProductName property
    /// </summary>
    public String DevName { get { return m_device.DeviceInformation.ProductName; } }


    /// <summary>
    /// ctor and init
    /// </summary>
    /// <param name="device">A DXInput device</param>
    /// <param name="hwnd">The WinHandle of the main window</param>
    /// <param name="panel">The respective JS panel to show the properties</param>
    public JoystickCls( Device device, Control hwnd, UC_JoyPanel panel )
    {
      m_device = device;
      m_hwnd = hwnd;
      m_jPanel = panel;

      m_jPanel.Caption = m_device.DeviceInformation.ProductName;

      // Set the data format to the c_dfDIJoystick pre-defined format.
      m_device.SetDataFormat( DeviceDataFormat.Joystick );
      // Set the cooperative level for the device.
      m_device.SetCooperativeLevel( m_hwnd, CooperativeLevelFlags.Exclusive | CooperativeLevelFlags.Foreground );
      // Enumerate all the objects on the device.
      foreach ( DeviceObjectInstance d in m_device.Objects ) {
        // For axes that are returned, set the DIPROP_RANGE property for the
        // enumerated axis in order to scale min/max values.
        if ( ( 0 != ( d.ObjectId & ( int )DeviceObjectTypeFlags.Axis ) ) ) {
          // Set the range for the axis.
          m_device.Properties.SetRange( ParameterHow.ById, d.ObjectId, new InputRange( -1000, +1000 ) );
        }
        // Update the controls to reflect what objects the device supports.
        UpdateControls( d );
      }

    }

    /// <summary>
    /// Shutdown device access
    /// </summary>
    public void FinishDX( )
    {
      if ( null != m_device )
        m_device.Unacquire( );
    }


    /// <summary>
    /// Enable the properties that are supported by the device
    /// </summary>
    /// <param name="d"></param>
    private void UpdateControls( DeviceObjectInstance d )
    {
      // Set the UI to reflect what objects the joystick supports.
      if ( ObjectTypeGuid.XAxis == d.ObjectType ) {
        m_jPanel.Xe = true;
      }
      if ( ObjectTypeGuid.YAxis == d.ObjectType ) {
        m_jPanel.Ye = true;
      }
      if ( ObjectTypeGuid.ZAxis == d.ObjectType ) {
        m_jPanel.Ze = true;
      }
      if ( ObjectTypeGuid.RxAxis == d.ObjectType ) {
        m_jPanel.Xre = true;
      }
      if ( ObjectTypeGuid.RyAxis == d.ObjectType ) {
        m_jPanel.Yre = true;
      }
      if ( ObjectTypeGuid.RzAxis == d.ObjectType ) {
        m_jPanel.Zre = true;
      }
      if ( ObjectTypeGuid.Slider == d.ObjectType ) {
        switch ( m_sliderCount++ ) {
          case 0:
            m_jPanel.S1e = true;
            break;

          case 1:
            m_jPanel.S2e = true;
            break;
        }
      }
      if ( ObjectTypeGuid.PointOfView == d.ObjectType ) {
        switch ( m_numPOVs++ ) {
          case 0:
            m_jPanel.H1e = true;
            break;

          case 1:
            m_jPanel.H2e = true;
            break;

          case 2:
            m_jPanel.H3e = true;
            break;

          case 3:
            m_jPanel.H4e = true;
            break;
        }
      }
    }

    /// <summary>
    /// Find the last change the user did on that device
    /// </summary>
    /// <returns>The last action as CryEngine compatible string</returns>
    public String GetLastChange( )
    {
      if ( m_state.X != m_prevState.X ) m_lastItem = "x";
      if ( m_state.Y != m_prevState.Y ) m_lastItem = "y";
      if ( m_state.Z != m_prevState.Z ) m_lastItem = "throttlez"; // this is not z because it usually maps the throttle 

      if ( m_state.Rx != m_prevState.Rx ) m_lastItem = "rotx";
      if ( m_state.Ry != m_prevState.Ry ) m_lastItem = "roty";
      if ( m_state.Rz != m_prevState.Rz ) m_lastItem = "rotz";

      int[] slider = m_state.GetSlider( );
      int[] pslider = m_prevState.GetSlider( );
      if ( slider[0] != pslider[0] ) m_lastItem = "slider1";
      if ( slider[1] != pslider[1] ) m_lastItem = "slider2";

      int[] pov = m_state.GetPointOfView( );
      int[] ppov = m_prevState.GetPointOfView( );
      if ( pov[0] >= 0 ) if ( pov[0] != ppov[0] ) m_lastItem = "hat1_" + HatDir( pov[0] );
      if ( pov[1] >= 0 ) if ( pov[1] != ppov[1] ) m_lastItem = "hat2_" + HatDir( pov[0] );
      if ( pov[2] >= 0 ) if ( pov[2] != ppov[2] ) m_lastItem = "hat3_" + HatDir( pov[0] );
      if ( pov[3] >= 0 ) if ( pov[3] != ppov[3] ) m_lastItem = "hat4_" + HatDir( pov[0] );

      byte[] buttons = m_state.GetButtons( );
      for ( int bi=0; bi < buttons.Length; bi++ ) {
        int b = buttons[bi] & 0x80;
        if ( b != 0 ) m_lastItem = "button" + ( bi + 1 ).ToString( );
      }
      return m_lastItem;
    }

    /// <summary>
    /// Show the current props in the GUI
    /// </summary>
    private void UpdateUI( )
    {
      // This function updated the UI with joystick state information.
      string strText = null;

      m_jPanel.X = m_state.X.ToString( );
      m_jPanel.Y = m_state.Y.ToString( );
      m_jPanel.Z = m_state.Z.ToString( );

      m_jPanel.Xr = m_state.Rx.ToString( );
      m_jPanel.Yr = m_state.Ry.ToString( );
      m_jPanel.Zr = m_state.Rz.ToString( );


      int[] slider = m_state.GetSlider( );

      m_jPanel.S1 = slider[0].ToString( );
      m_jPanel.S2 = slider[1].ToString( );

      int[] pov = m_state.GetPointOfView( );

      m_jPanel.H1 = pov[0].ToString( );
      m_jPanel.H2 = pov[1].ToString( );
      m_jPanel.H3 = pov[2].ToString( );
      m_jPanel.H4 = pov[3].ToString( );

      // Fill up text with which buttons are pressed
      byte[] buttons = m_state.GetButtons( );

      int button = 0;
      foreach ( byte b in buttons ) {
        if ( 0 != ( b & 0x80 ) )
          strText += ( button + 1 ).ToString( "00 " ); // buttons are 1 based
        button++;
      }
      m_jPanel.Button = strText;
    }


    /// <summary>
    /// Collect the current data from the device
    /// </summary>
    public void GetData( )
    {
      // Make sure there is a valid device.
      if ( null == m_device )
        return;

      try {
        // Poll the device for info.
        m_device.Poll( );
      }
      catch ( InputException inputex ) {
        if ( ( inputex is NotAcquiredException ) || ( inputex is InputLostException ) ) {
          // Check to see if either the app needs to acquire the device, or
          // if the app lost the device to another process.
          try {
            // Acquire the device.
            m_device.Acquire( );
          }
          catch ( InputException ) {
            // Failed to acquire the device.
            // This could be because the app doesn't have focus.
            return;
          }
        }

      } //catch(InputException inputex)

      // Get the state of the device - retaining the previous state to find the lates change
      m_prevState = m_state;
      try { m_state = m_device.CurrentJoystickState; }
      // Catch any exceptions. None will be handled here, 
      // any device re-aquisition will be handled above.  
      catch ( InputException ) {
        return;
      }

      UpdateUI( ); // and update the GUI
    }



  }
}

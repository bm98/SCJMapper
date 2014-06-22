﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Joystick
{
  /// <summary>
  /// should read the default profile - may be replacing the MappingVars once
  /// </summary>
  class DProfileReader
  {
    public Boolean ValidContent { get; set; }

    private Stack<String> m_nodeNameStack = null; // element name stack - keeping track where we are

    // state for the parser
    enum EState
    {
      idle = 0,
      inActionMap,
    }
    private EState m_state = EState.idle;

    // an action map and its actions
    class Action
    {
      public String name { get; set; }    // the action name
      public String input { get; set; }  // the input method K,J,X,P
      public String defBinding { get; set; }  // the default binding - may be useful
      public String keyName
      { get { return input + name; } } // prep for TreView usage - create a key from input+name
    }
    class ActionMap : List<Action>  // carries the action list
    {
      public String name { get; set; } // the map name
    };
    Dictionary<String, ActionMap> m_aMap = null; // key would be the actionmap name
    ActionMap m_currentMap = null;

    // actionmap names to gather (do we need them to be cofigurable ??)
    private String[] c_exMaps = { "spaceship_general", "spaceship_view", "spaceship_movement", "spaceship_targeting", "spaceship_weapons", "spaceship_missiles", 
                                "spaceship_defensive", "spaceship_auto_weapons", "spaceship_radar" , "spaceship_hud" , "IFCS_controls" , "" , "" , "" };


    public DProfileReader( )
    {
      ValidContent = false; // default
    }


    /// <summary>
    /// Returns the collected actionmaps as CSV (same format as MappingVars)
    /// i.e. one line per actionmap where the first element is the actionmap and following are actions lead by the input Key in uppercase (JKXP)
    /// </summary>
    public String CSVMap
    {
      get
      {
        String buf = "";
        foreach ( ActionMap am in m_aMap.Values ) {
          buf += am.name + ";";
          foreach ( Action a in am ) {
            buf += a.keyName + ";";
          }
          buf += String.Format( "\n" );
        }
        return buf;
      }
    }


    /// <summary>
    /// Read one 'empty' XML element
    /// 
    /// <name [attr="" ..] />
    /// 
    /// </summary>
    /// <param name="xr">An XML reader @ StartElement</param>
    /// <returns>True if reading can continue</returns>
    private Boolean ReadEmptyElement( XmlReader xr )
    {
      Dictionary<String, String> attr = new Dictionary<string, string>( );
      String eName = xr.Name;
      switch ( xr.NodeType ) {
        case XmlNodeType.Element:
          //Console.Write( "<{0}", xr.Name );
          while ( xr.MoveToNextAttribute( ) ) {
            attr.Add( xr.Name, xr.Value );  // save the attributes
            //Console.Write( " {0}='{1}'", xr.Name, xr.Value );
          }
          if ( m_state == EState.inActionMap ) {
            // processing a valid action map - collect actions
            if ( eName.ToLower( ) == "action" ) {
              // this is an action.. - collect it
              // we collect actions for each input ie for K,J,X and P
              if ( attr.ContainsKey( "joystick" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "J";
                ac.defBinding = attr["joystick"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
              if ( attr.ContainsKey( "keyboard" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "K";
                ac.defBinding = attr["keyboard"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
              if ( attr.ContainsKey( "xboxpad" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "X";
                ac.defBinding = attr["xboxpad"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
              if ( attr.ContainsKey( "ps3pad" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "P";
                ac.defBinding = attr["ps3pad"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }

            }
          }// if inmap
          //Console.Write( ">\n" );
          m_nodeNameStack.Pop( );

          break;
        case XmlNodeType.Text:
          //Console.Write( xr.Value );
          break;
        case XmlNodeType.EndElement:
          //Console.Write( "</{0}>\n", xr.Name );
          break;
      }

      return true;
    }

    /// <summary>
    /// Read one XML element 
    /// 
    /// <name attr="">
    ///   [ Xelement ]
    /// </name>
    /// 
    /// </summary>
    /// <param name="xr">An XML reader @ StartElement</param>
    /// <returns>True if reading can continue</returns>
    private Boolean ReadElement( XmlReader xr )
    {
      Dictionary<String, String> attr = new Dictionary<string, string>( );
      String eName = xr.Name;
      switch ( xr.NodeType ) {
        case XmlNodeType.Element:
          //Console.Write( "<{0}", xr.Name );
          while ( xr.MoveToNextAttribute( ) ) {
            attr.Add( xr.Name, xr.Value );  // save the attributes
            //Console.Write( " {0}='{1}'", xr.Name, xr.Value );
          }
          // now here we could have an actionmap start
          if ( m_state == EState.idle ) {
            if ( m_nodeNameStack.Peek( ).ToLower( ) == "actionmap" ) {
              // check for a valid one
              String mapName = attr["name"];
              String item = Array.Find( c_exMaps, delegate( String sstr ) {
                return sstr == mapName;
              } );
              if ( !String.IsNullOrEmpty( item ) ) {
                // finally.... it is a valid actionmap
                m_currentMap = new ActionMap( );
                m_currentMap.name = mapName;
                m_aMap.Add( mapName, m_currentMap ); // add to our inventory
                m_state = EState.inActionMap; // now we are in and processing the map
              }
            }
          }
          else if ( m_state == EState.inActionMap ) {
            // processing a valid action map - collect actions
            if ( eName.ToLower( ) == "action" ) {
              // this is an action.. - collect it
              // we collect actions for each input ie for K,J,X and P
              if ( attr.ContainsKey( "joystick" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "J";
                ac.defBinding = attr["joystick"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
              if ( attr.ContainsKey( "keyboard" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "K";
                ac.defBinding = attr["keyboard"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
              if ( attr.ContainsKey( "xboxpad" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "X";
                ac.defBinding = attr["xboxpad"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
              if ( attr.ContainsKey( "ps3pad" ) ) {
                Action ac = new Action( );
                ac.name = attr["name"];
                ac.input = "P";
                ac.defBinding = attr["ps3pad"];
                if ( !String.IsNullOrEmpty( ac.defBinding ) ) m_currentMap.Add( ac );  // finally add it to the current map if it was bound
              }
            }
          }
          //Console.Write( ">\n" );
          break;
        case XmlNodeType.Text:
          //Console.Write( xr.Value );
          break;
        case XmlNodeType.EndElement:
          //Console.Write( "</{0}>\n", xr.Name );
          break;
      }
      return true;
    }


    /// <summary>
    /// Read the xml part
    /// </summary>
    /// <param name="xr"></param>
    /// <returns></returns>
    private Boolean ReadXML( XmlReader xr )
    {
      Boolean retVal = true;

      do {
        if ( xr.IsStartElement( ) ) {
          m_nodeNameStack.Push( xr.Name );
          if ( xr.IsEmptyElement )
            retVal = retVal && ReadEmptyElement( xr );
          else {
            retVal = retVal && ReadElement( xr );
          }
        }
        else if ( xr.NodeType == XmlNodeType.EndElement ) {
          //Console.Write( "</{0}>\n", xr.Name );
          String exitElement = m_nodeNameStack.Pop( );
          if ( m_state == EState.inActionMap )
            if ( exitElement.ToLower( ) == "actionmap" ) m_state = EState.idle; // finished 
        }

      } while ( xr.Read( ) );

      if ( m_nodeNameStack.Count == 0 )
        return retVal && true;
      else
        return false;
    }

    /// <summary>
    /// Read the defaultProfile.xml - do some sanity check
    /// </summary>
    /// <param name="xml">the XML action defaultProfile Content</param>
    /// <returns>True if an action was decoded</returns>
    public Boolean fromXML( String xml )
    {
      XmlReaderSettings settings = new XmlReaderSettings( );
      settings.ConformanceLevel = ConformanceLevel.Fragment;
      settings.IgnoreWhitespace = true;
      settings.IgnoreComments = true;
      XmlReader reader = XmlReader.Create( new StringReader( xml ), settings );

      m_nodeNameStack = new Stack<String>( );
      m_aMap = new Dictionary<String, ActionMap>( );

      reader.Read( );
      ValidContent = ReadXML( reader );

      return ValidContent;
    }




  }
}

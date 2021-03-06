/*
�--------------------------------------------------------------------�
| ReportingCloud - Engine                                            |
| Copyright (c) 2010, FlexibleCoder.                                 |
| https://sourceforge.net/projects/reportingcloud                    |
�--------------------------------------------------------------------�
| This library is free software; you can redistribute it and/or      |
| modify it under the terms of the GNU Lesser General Public         |
| License as published by the Free Software Foundation; either       |
| version 2.1 of the License, or (at your option) any later version. |
|                                                                    |
| This library is distributed in the hope that it will be useful,    |
| but WITHOUT ANY WARRANTY; without even the implied warranty of     |
| MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU  |
| Lesser General Public License for more details.                    |
|                                                                    |
| GNU LGPL: http://www.gnu.org/copyleft/lesser.html                  |
�--------------------------------------------------------------------�
*/

using System;
using System.Xml;

namespace ReportingCloud.Engine
{
	///<summary>
	/// Matrix row grouping definition.
	///</summary>
	[Serializable]
	internal class RowGrouping : ReportLink
	{
		RSize _Width;	// Width of the row header
		DynamicRows _DynamicRows;	// Dynamic row headings for this grouping
		StaticRows _StaticRows;	// Static row headings for this grouping		
	
		internal RowGrouping(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Width=null;
			_DynamicRows=null;
			_StaticRows=null;

			// Loop thru all the child nodes
			foreach(XmlNode xNodeLoop in xNode.ChildNodes)
			{
				if (xNodeLoop.NodeType != XmlNodeType.Element)
					continue;
				switch (xNodeLoop.Name)
				{
					case "Width":
						_Width = new RSize(r, xNodeLoop);
						break;
					case "DynamicRows":
						_DynamicRows = new DynamicRows(r, this, xNodeLoop);
						break;
					case "StaticRows":
						_StaticRows = new StaticRows(r, this, xNodeLoop);
						break;
					default:	
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown RowGrouping element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}
			if (_Width == null)
				OwnerReport.rl.LogError(8, "RowGrouping requires the Width element.");
		}
		
		override internal void FinalPass()
		{
			if (_DynamicRows != null)
				_DynamicRows.FinalPass();
			if (_StaticRows != null)
				_StaticRows.FinalPass();
			return;
		}

		internal RSize Width
		{
			get { return  _Width; }
			set {  _Width = value; }
		}

		internal DynamicRows DynamicRows
		{
			get { return  _DynamicRows; }
			set {  _DynamicRows = value; }
		}

		internal StaticRows StaticRows
		{
			get { return  _StaticRows; }
			set {  _StaticRows = value; }
		}
	}
}

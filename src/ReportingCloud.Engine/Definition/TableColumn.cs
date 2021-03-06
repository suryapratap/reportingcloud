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
using System.IO;

namespace ReportingCloud.Engine
{
	///<summary>
	/// TableColumn definition and processing.
	///</summary>
	[Serializable]
	internal class TableColumn : ReportLink
	{
		RSize _Width;		// Width of the column
		Visibility _Visibility;	// Indicates if the column should be hidden	
		bool _FixedHeader=false;	// Header of this column should be display even when scrolled
	
		internal TableColumn(ReportDefn r, ReportLink p, XmlNode xNode) : base(r, p)
		{
			_Width=null;
			_Visibility=null;

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
					case "Visibility":
						_Visibility = new Visibility(r, this, xNodeLoop);
						break;
					case "FixedHeader":
						_FixedHeader = XmlUtil.Boolean(xNodeLoop.InnerText, OwnerReport.rl);
						break;
					default:
						// don't know this element - log it
						OwnerReport.rl.LogError(4, "Unknown TableColumn element '" + xNodeLoop.Name + "' ignored.");
						break;
				}
			}
			if (_Width == null)
				OwnerReport.rl.LogError(8, "TableColumn requires the Width element.");
		}
		
		override internal void FinalPass()
		{
			if (_Visibility != null)
				_Visibility.FinalPass();
			return;
		}

		internal void Run(IPresent ip, Row row)
		{
		}

		internal RSize Width
		{
			get { return  _Width; }
			set {  _Width = value; }
		}

		internal float GetXPosition(Report rpt)
		{
			WorkClass wc = GetWC(rpt);
			return wc.XPosition;
		}

		internal void SetXPosition(Report rpt, float xp)
		{
			WorkClass wc = GetWC(rpt);
			wc.XPosition = xp;
		}

		internal Visibility Visibility
		{
			get { return  _Visibility; }
			set {  _Visibility = value; }
		}

		internal bool IsHidden(Report rpt, Row r)
		{
			if (_Visibility == null)
				return false;
			return _Visibility.IsHidden(rpt, r);
		}

		private WorkClass GetWC(Report rpt)
		{
			if (rpt == null)	
				return new WorkClass();

			WorkClass wc = rpt.Cache.Get(this, "wc") as WorkClass;
			if (wc == null)
			{
				wc = new WorkClass();
				rpt.Cache.Add(this, "wc", wc);
			}
			return wc;
		}

		private void RemoveWC(Report rpt)
		{
			rpt.Cache.Remove(this, "wc");
		}

		class WorkClass
		{
			internal float XPosition;	// Set at runtime by Page processing; potentially dynamic at runtime
			//  since visibility is an expression
			internal WorkClass()
			{
				XPosition=0;
			}
		}
	}
}

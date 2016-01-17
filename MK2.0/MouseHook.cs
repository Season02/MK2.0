// /////////////////////////////////////////////////////////////
// File: MouseHook.cs		Class: Kennedy.ManagedHooks.MouseHook
// Date: 2/25/2004			Author: Michael Kennedy
// Language: C#				Framework: .NET
//
// Copyright: Copyright (c) Michael Kennedy, 2004-2005
// /////////////////////////////////////////////////////////////
// License: See License.txt file included with application.
// Description: See compiled documentation (Managed Hooks.chm)
// /////////////////////////////////////////////////////////////

using System;
using System.Drawing;

namespace Kennedy.ManagedHooks
{
	/// <include file='ManagedHooks.xml' path='Docs/MouseEvents/enum/*'/>
	public enum MouseEvents
	{
		/// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
		LeftButtonDown	= 0x0201,
		/// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
		LeftButtonUp	= 0x0202,
		/// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
		Move			= 0x0200,
		/// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
		MouseWheel		= 0x020A,
		/// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
		RightButtonDown = 0x0204,
		/// <include file='ManagedHooks.xml' path='Docs/General/Empty/*'/>
		RightButtonUp	= 0x0205
	}

	/// <include file='ManagedHooks.xml' path='Docs/MouseHook/Class/*'/>
	public class MouseHook : SystemHook
	{
		/// <include file='ManagedHooks.xml' path='Docs/MouseHook/MouseEventHandler/*'/>
		public delegate void MouseEventHandler(MouseEvents mEvent, Point point);

		/// <include file='ManagedHooks.xml' path='Docs/MouseHook/MouseEvent/*'/>
		public event MouseEventHandler MouseEvent;

		/// <include file='ManagedHooks.xml' path='Docs/MouseHook/ctor/*'/>
		public MouseHook() : base(HookTypes.MouseLL)
		{
		}

		/// <include file='ManagedHooks.xml' path='Docs/MouseHook/HookCallback/*'/>
		protected override void HookCallback(int code, UIntPtr wparam, IntPtr lparam)
		{
			if (MouseEvent == null)
			{
				return;
			}

			int x = 0, y = 0;
			MouseEvents mEvent = (MouseEvents)wparam.ToUInt32();

			switch(mEvent)
			{
				case MouseEvents.LeftButtonDown:
					GetMousePosition(wparam, lparam, ref x, ref y);
					break;
				case MouseEvents.LeftButtonUp:
					GetMousePosition(wparam, lparam, ref x, ref y);
					break;
				case MouseEvents.MouseWheel:
					break;
				case MouseEvents.Move:
					GetMousePosition(wparam, lparam, ref x, ref y);
					break;
				case MouseEvents.RightButtonDown:
					GetMousePosition(wparam, lparam, ref x, ref y);
					break;
				case MouseEvents.RightButtonUp:
					GetMousePosition(wparam, lparam, ref x, ref y);
					break;
				default:
					//System.Diagnostics.Trace.WriteLine("Unrecognized mouse event");
					break;
			}

			MouseEvent(mEvent, new Point(x, y));
		}

		/// <include file='ManagedHooks.xml' path='Docs/MouseHook/FilterMessage/*'/>
		public void FilterMessage(MouseEvents eventType)
		{
			base.FilterMessage(this.HookType, (int)eventType);
		}

	}
}

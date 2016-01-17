// /////////////////////////////////////////////////////////////
// File: SystemHook.cs		Class: Kennedy.ManagedHooks.SystemHook
// Date: 2/25/2004			Author: Michael Kennedy
// Language: C#				Framework: .NET
//
// Copyright: Copyright (c) Michael Kennedy, 2004-2005
// /////////////////////////////////////////////////////////////
// License: See License.txt file included with application.
// Description: See compiled documentation (Managed Hooks.chm)
// /////////////////////////////////////////////////////////////


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Kennedy.ManagedHooks
{
	/// <include file='ManagedHooks.xml' path='Docs/SystemHook/Class/*'/>
	public abstract class SystemHook : IDisposable
	{
		#region Member Variables and Delegates

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookProcessedHandler/*'/>
		protected delegate void HookProcessedHandler(int code, UIntPtr wparam, IntPtr lparam);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/type/*'/>
		private HookTypes _type = HookTypes.None;

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/processHandler/*'/>
		private HookProcessedHandler _processHandler = null;

		private bool _isHooked = false;

		#endregion

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/ctor/*'/>
		public SystemHook(HookTypes type)
		{
			_type = type;

			_processHandler = new HookProcessedHandler(InternalHookCallback);
			SetCallBackResults result = SetUserHookCallback(_processHandler, _type);
			if (result != SetCallBackResults.Success)
			{
				this.Dispose();
				GenerateCallBackException(type, result);
			}
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/dtor/*'/>
		~SystemHook()
		{
			Trace.WriteLine("SystemHook (" + _type + ") WARNING: Finalizer called, " +
				"a reference has not been properly disposed.");

			Dispose(false);
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookCallback/*'/>
		protected abstract void HookCallback(int code, UIntPtr wparam, IntPtr lparam);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/InternalHookCallback/*'/>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void InternalHookCallback(int code, UIntPtr wparam, IntPtr lparam)
		{
			try
			{
				HookCallback(code, wparam, lparam);
			}
			catch (Exception e)
			{
				//
				// While it is not generally a good idea to trap and discard all exceptions
				// in a base class, this is a special case. Remember, this is the entry point
				// for the C++ library to call into our .NET code. We don't want to return
				// .NET exceptions to C++. If it gets this far all we can do is drop them.
				//
				Debug.WriteLine("Exception during hook callback: " + e.Message + " " + e.ToString());
			}
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/InstallHook/*'/>
		public void InstallHook()
		{
			if (!InitializeHook(_type, 0))
			{
				throw new ManagedHooksException("Hook initialization failed.");
			}
			_isHooked = true;
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/UninstallHook/*'/>
		public void UninstallHook()
		{
			_isHooked = false;
			UninitializeHook(_type);
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/HookType/*'/>
		protected HookTypes HookType
		{
			get
			{
				return _type;
			}
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/IsHooked/*'/>
		public bool IsHooked
		{
			get
			{
				return _isHooked;
			}
		}

		#region Exception Generation Helper Methods

		private void GenerateCallBackException(HookTypes type, SetCallBackResults result)
		{
			if (result == SetCallBackResults.Success)
			{
				return;
			}

			string msg;

			switch (result)
			{
				case SetCallBackResults.AlreadySet:
					msg = "A hook of type " + type + " is already registered. You can only register ";
					msg += "a single instance of each type of hook class. This can also occur when you forget ";
					msg += "to unregister or dispose a previous instance of the class.";

					throw new ManagedHooksException(msg);

				case SetCallBackResults.ArgumentError:
					msg = "Failed to set hook callback due to an error in the arguments.";

					throw new ArgumentException(msg);

				case SetCallBackResults.NotImplemented:
					msg = "The hook type of type " + type + " is not implemented in the C++ layer. ";
					msg += "You must implement this hook type before you can use it. See the C++ function ";
					msg += "SetUserHookCallback.";

					throw new HookTypeNotImplementedException(msg);
			}

			msg = "Unrecognized exception during hook callback setup. Error code " + result + ".";
			throw new ManagedHooksException(msg);
		}

		private void GenerateFilterMessageException(HookTypes type, FilterMessageResults result)
		{
			if (result == FilterMessageResults.Success)
			{
				return;
			}

			string msg;

			if (result == FilterMessageResults.NotImplemented)
			{
					msg = "The hook type of type " + type + " is not implemented in the C++ layer. ";
					msg += "You must implement this hook type before you can use it. See the C++ function ";
					msg += "FilterMessage.";

					throw new HookTypeNotImplementedException(msg);
			}

			//
			// All other errors are general errors.
			//
			msg = "Unrecognized exception during hook FilterMessage call. Error code " + result + ".";
			throw new ManagedHooksException(msg);
		}

		#endregion

		#region IDisposable Members

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/Dispose/*'/>
		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}

			UninstallHook();
			DisposeCppLayer(_type);
		}

		#endregion

		#region Win32 API Utility Methods

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/GetMousePosition/*'/>
		protected void GetMousePosition(UIntPtr wparam, IntPtr lparam, ref int x, ref int y)
		{
			if (!InternalGetMousePosition(wparam, lparam, ref x, ref y))
			{
				throw new ManagedHooksException("Failed to access mouse position.");
			}
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/GetKeyboardReading/*'/>
		protected void GetKeyboardReading(UIntPtr wparam, IntPtr lparam, ref int vkCode)
		{
			if (!IntenralGetKeyboardReading(wparam, lparam, ref vkCode))
			{
				throw new ManagedHooksException("Failed to access keyboard settings.");
			}
		}

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/FilterMessage/*'/>
		protected void FilterMessage(HookTypes hookType, int message)
		{
			FilterMessageResults result = InternalFilterMessage(hookType, message);
			if (result != FilterMessageResults.Success)
			{
				GenerateFilterMessageException(hookType, result);
			}
		}

		#endregion

		#region Imported Static Methods for SystemHookCore.dll

		private enum SetCallBackResults
		{
			Success			= 1,
			AlreadySet		= -2,
			NotImplemented	= -3,
			ArgumentError	= -4
		};

		private enum FilterMessageResults
		{
			Success			= 1,
			Failed			= -2,
			NotImplemented	= -3
		};
        
		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/SetUserHookCallback/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="SetUserHookCallback",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern SetCallBackResults SetUserHookCallback(HookProcessedHandler hookCallback, HookTypes hookType);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/InitializeHook/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="InitializeHook",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern bool InitializeHook(HookTypes hookType, int threadID);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/UninitializeHook/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="UninitializeHook",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern void UninitializeHook(HookTypes hookType);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/InternalGetMousePosition/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="GetMousePosition",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern bool InternalGetMousePosition(UIntPtr wparam, IntPtr lparam, ref int x, ref int y);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/IntenralGetKeyboardReading/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="GetKeyboardReading",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern bool IntenralGetKeyboardReading(UIntPtr wparam, IntPtr lparam, ref int vkCode);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/DisposeCppLayer/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="Dispose",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern void DisposeCppLayer(HookTypes hookType);

		/// <include file='ManagedHooks.xml' path='Docs/SystemHook/InternalFilterMessage/*'/>
		[DllImport("SystemHookCore.dll", EntryPoint="FilterMessage",  SetLastError=true,
			 CharSet=CharSet.Unicode, ExactSpelling=true,
			 CallingConvention=CallingConvention.StdCall)]
		private static extern FilterMessageResults InternalFilterMessage(HookTypes hookType, int message);

		#endregion

	}
}

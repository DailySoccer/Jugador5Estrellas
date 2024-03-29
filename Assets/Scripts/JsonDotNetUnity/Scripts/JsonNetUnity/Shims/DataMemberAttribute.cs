﻿//
// DataMemberAttribute.cs
//
// Author:
//	Atsushi Enomoto <atsushi@ximian.com>
//
// Copyright (C) 2005 Novell, Inc.  http://www.novell.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Runtime.Serialization
{
	[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field,
		Inherited = false, AllowMultiple = false)]
	public sealed class DataMemberAttribute : Attribute
	{
		bool is_required;
		bool emit_default = true;
		string name;
		int order = -1;

		public DataMemberAttribute ()
		{
		}

		public bool EmitDefaultValue {
			get { return emit_default; }
			set { emit_default = value; }
		}

		public bool IsRequired {
			get { return is_required; }
			set { is_required = value; }
		}

		public string Name {
			get { return name; }
			set { name = value; }
		}

		public int Order {
			get { return order; }
			set { order = value; }
		}
	}
}
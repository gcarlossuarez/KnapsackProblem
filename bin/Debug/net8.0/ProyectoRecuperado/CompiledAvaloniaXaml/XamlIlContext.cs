using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.XamlIl.Runtime;

namespace CompiledAvaloniaXaml;

internal class XamlIlContext
{
	public class Context<TTarget> : IRootObjectProvider, IAvaloniaXamlIlParentStackProvider, ITypeDescriptorContext, IProvideValueTarget, IUriContext, IServiceProvider, IAvaloniaXamlIlEagerParentStackProvider
	{
		public TTarget RootObject;

		public object IntermediateRoot;

		private IServiceProvider _sp;

		private IServiceProvider _innerSp;

		private object[] _staticProviders;

		public List<object> ParentsStack;

		private IEnumerable<object> _parentStackEnumerable;

		public object ProvideTargetObject;

		public object ProvideTargetProperty;

		private Uri _baseUri;

		public INameScope AvaloniaNameScope;

		virtual object IRootObjectProvider.RootObject
		{
			get
			{
				if (RootObject != null)
				{
					return RootObject;
				}
				if (_sp != null)
				{
					IRootObjectProvider rootObjectProvider = (IRootObjectProvider)_sp.GetService(typeof(IRootObjectProvider));
					if (rootObjectProvider != null)
					{
						return rootObjectProvider.RootObject;
					}
				}
				return null;
			}
		}

		virtual object IRootObjectProvider.IntermediateRootObject => IntermediateRoot;

		virtual IEnumerable<object> IAvaloniaXamlIlParentStackProvider.Parents => _parentStackEnumerable;

		virtual IContainer ITypeDescriptorContext.Container => null;

		virtual object ITypeDescriptorContext.Instance => null;

		virtual PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor => null;

		virtual object IProvideValueTarget.TargetObject => ProvideTargetObject;

		virtual object IProvideValueTarget.TargetProperty => ProvideTargetProperty;

		public virtual Uri BaseUri
		{
			get
			{
				return _baseUri;
			}
			set
			{
				_baseUri = baseUri;
			}
		}

		private virtual IReadOnlyList<object> DirectParentsStack => ParentsStack;

		private virtual IAvaloniaXamlIlEagerParentStackProvider ParentProvider => ((IAvaloniaXamlIlParentStackProvider)_sp.GetService(typeof(IAvaloniaXamlIlParentStackProvider))).AsEagerParentStackProvider();

		virtual bool ITypeDescriptorContext.OnComponentChanging()
		{
			throw new NotSupportedException();
		}

		virtual void ITypeDescriptorContext.OnComponentChanged()
		{
			throw new NotSupportedException();
		}

		public virtual object GetService(Type P_0)
		{
			if (_innerSp != null)
			{
				object service = _innerSp.GetService(P_0);
				if (service != null)
				{
					return service;
				}
			}
			if (typeof(IRootObjectProvider).Equals(P_0))
			{
				return this;
			}
			if (typeof(IAvaloniaXamlIlParentStackProvider).Equals(P_0))
			{
				return this;
			}
			if (typeof(ITypeDescriptorContext).Equals(P_0))
			{
				return this;
			}
			if (typeof(IProvideValueTarget).Equals(P_0))
			{
				return this;
			}
			if (typeof(IUriContext).Equals(P_0))
			{
				return this;
			}
			if (_staticProviders != null)
			{
				for (int i = 0; i < (nint)_staticProviders.LongLength; i++)
				{
					object obj = _staticProviders[i];
					if (P_0.IsAssignableFrom(obj.GetType()))
					{
						return obj;
					}
				}
			}
			if (_sp != null)
			{
				return _sp.GetService(P_0);
			}
			return null;
		}

		public Context(IServiceProvider P_0, object[] P_1, string P_2)
		{
			_sp = P_0;
			_staticProviders = P_1;
			if (P_2 != null)
			{
				_baseUri = new Uri(P_2);
			}
			ParentsStack = new List<object>();
			_parentStackEnumerable = new ParentStackEnumerable(ParentsStack, _sp);
			AvaloniaNameScope = (INameScope)P_0.GetService(typeof(INameScope));
			_innerSp = XamlIlRuntimeHelpers.CreateInnerServiceProviderV1(this);
		}

		public void PushParent(object P_0)
		{
			ParentsStack.Add(P_0);
			ProvideTargetObject = P_0;
		}

		public void PopParent()
		{
			int index = ParentsStack.Count - 1;
			ProvideTargetObject = ParentsStack[index];
			ParentsStack.RemoveAt(index);
		}
	}

	private class ParentStackEnumerable : IEnumerable<object>
	{
		public class Enumerator : IEnumerator<object>
		{
			private int _state;

			private List<object> _parentList;

			private IServiceProvider _parentSP;

			private List<object> _list;

			private int _listIndex;

			private object _current;

			private IEnumerator<object> _parentEnumerator;

			public virtual object Current => _current;

			public Enumerator(List<object> P_0, IServiceProvider P_1)
			{
				_parentList = P_0;
				_parentSP = P_1;
			}

			virtual void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			virtual void IDisposable.Dispose()
			{
				if (_parentEnumerator != null)
				{
					_parentEnumerator.Dispose();
				}
			}

			virtual bool IEnumerator.MoveNext()
			{
				if (_state != 0)
				{
					if (_state != 1)
					{
						if (_state != 2)
						{
							return false;
						}
						goto IL_00c8;
					}
				}
				else
				{
					_list = _parentList;
					_listIndex = _list.Count - 1;
					_state = 1;
				}
				if (_listIndex >= 0)
				{
					_current = _list[_listIndex];
					_listIndex--;
					return true;
				}
				IAvaloniaXamlIlParentStackProvider avaloniaXamlIlParentStackProvider;
				if (_parentSP != null && (avaloniaXamlIlParentStackProvider = (IAvaloniaXamlIlParentStackProvider)_parentSP.GetService(typeof(IAvaloniaXamlIlParentStackProvider))) != null)
				{
					_parentEnumerator = avaloniaXamlIlParentStackProvider.Parents.GetEnumerator();
					_state = 2;
					goto IL_00c8;
				}
				goto IL_00eb;
				IL_00eb:
				_state = 3;
				return false;
				IL_00c8:
				if (_parentEnumerator.MoveNext())
				{
					_current = _parentEnumerator.Current;
					return true;
				}
				goto IL_00eb;
			}
		}

		private List<object> _parentList;

		private IServiceProvider _parentSP;

		public ParentStackEnumerable(List<object> P_0, IServiceProvider P_1)
		{
			_parentList = P_0;
			_parentSP = P_1;
		}

		public virtual IEnumerator<object> GetEnumerator()
		{
			return new Enumerator(_parentList, _parentSP);
		}

		virtual IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

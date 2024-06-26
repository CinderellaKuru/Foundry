﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif
using Exprs = System.Linq.Expressions;

namespace KSoft.ObjectModel
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1724:TypeNamesShouldNotMatchNamespaces",
		Justification="I don't care about System.Web.Util")]
	public static class Util
	{
		// based on System.Windows.Data.Binding.IndexerName in PresentationFramework.dll
		public const string kIndexerPropertyName = "Item[]";
		public static readonly PropertyChangedEventArgs kIndexerPropertyChanged =
			new PropertyChangedEventArgs(kIndexerPropertyName);


		public static readonly System.Collections.Specialized.NotifyCollectionChangedEventArgs kNotifyCollectionReset =
			new System.Collections.Specialized.NotifyCollectionChangedEventArgs(
				System.Collections.Specialized.NotifyCollectionChangedAction.Reset);

		public static PropertyChangedEventArgs CreatePropertyChangedEventArgs<T>(
			Exprs.Expression<Func<T, object>> propertyExpr)
		{
			Contract.Requires(propertyExpr != null);
			Contract.Requires(propertyExpr.Body is Exprs.MemberExpression || propertyExpr.Body is Exprs.UnaryExpression);

			return new PropertyChangedEventArgs(
				Reflection.Util.PropertyFromExpr(propertyExpr).Name);
		}
		public static PropertyChangedEventArgs CreatePropertyChangedEventArgs<T, TProp>(
			Exprs.Expression<Func<T, TProp>> propertyExpr)
		{
			Contract.Requires(propertyExpr != null);
			Contract.Requires(propertyExpr.Body is Exprs.MemberExpression || propertyExpr.Body is Exprs.UnaryExpression);

			return new PropertyChangedEventArgs(
				Reflection.Util.PropertyFromExpr(propertyExpr).Name);
		}

		private static Dictionary<Type, Func<object, object>> gCollectionGetUnderlyingListFuncs = new Dictionary<Type, Func<object, object>>();
		public static List<T> GetUnderlyingItemsAsList<T>(System.Collections.ObjectModel.Collection<T> coll, bool throwOnError = true)
		{
			if (coll == null)
				return null;

			var collType = coll.GetType();
			Func<object, object> getFunc;
			lock (gCollectionGetUnderlyingListFuncs)
			{
				if (!gCollectionGetUnderlyingListFuncs.TryGetValue(collType, out getFunc))
				{
					getFunc = Reflection.Util.GenerateMemberGetter<object>(collType, "items");
					gCollectionGetUnderlyingListFuncs.Add(collType, getFunc);
				}
			}

			var items = getFunc(coll);
			var list = items as List<T>;

			if (list == null && throwOnError)
			{
				throw new InvalidOperationException(string.Format(KSoft.Util.InvariantCultureInfo,
					"Tried to get Collection's underling Items as a List<{0}> but it is a {1}",
					typeof(T), items?.GetType()));
			}

			return list;
		}
	};
}

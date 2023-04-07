using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Template.Application.Tests;

public static class TestExtensions
{
    public static void SetPrivatePropertyValue<T>(this T obj, string propName, object value)
    {
        obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value, null);
    }

    public static TEntity WithId<TEntity>(this TEntity entity, int id) where TEntity : BaseEntity
    {
        typeof(TEntity).GetProperty("Id").SetValue(entity, id);
        return entity;
    }
}
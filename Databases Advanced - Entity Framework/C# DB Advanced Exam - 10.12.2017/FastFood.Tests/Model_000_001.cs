using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FastFood.Data;
using FastFood.Models;

[TestFixture]
public class Model_000_001
{
    private static readonly Assembly CurrentAssembly = typeof(Employee).Assembly;

    [Test]
    public void ValidateModel()
    {
        var context = typeof(FastFoodDbContext);

        var dbSetData = new[]
        {
            new DbSetInfo("Employees", "Employee", "Id Name Age PositionId Position Orders".Split()),
            new DbSetInfo("Positions", "Position", "Id Name Employees".Split()),
            new DbSetInfo("Categories", "Category", "Id Name Items".Split()),
            new DbSetInfo("Items", "Item", "Id Name CategoryId Category Price OrderItems".Split()),
            new DbSetInfo("Orders", "Order", "Id Customer DateTime Type TotalPrice EmployeeId Employee OrderItems".Split()),
            new DbSetInfo("OrderItems", "OrderItem", "OrderId Order ItemId Item".Split()),
        };

        foreach (var dbSetInfo in dbSetData)
        {
            ValidateDbSet(context, dbSetInfo.DbSetName, dbSetInfo.EntityName, dbSetInfo.Properties);
        }
    }

    private static void ValidateDbSet(Type context, string dbSetName, string modelName, IEnumerable<string> properties)
    {
        var expectedDbSetType = GetDbSetType(modelName);

        AssertCollectionIsOfType(context, dbSetName, expectedDbSetType);

        var modelType = GetModelType(modelName);

        foreach (var property in properties)
        {
            var propertyType = GetPropertyByName(modelType, property);

            var errorMessage = $"{modelType.Name}.{property} property does not exist!";
            Assert.IsNotNull(propertyType, errorMessage);
        }
    }

    private static PropertyInfo GetPropertyByName(Type type, string propName)
    {
        var properties = type.GetProperties();

        var firstOrDefault = properties.FirstOrDefault(p => p.Name == propName);
        return firstOrDefault;
    }

    private static void AssertCollectionIsOfType(Type type, string propertyName, Type collectionType)
    {
        var property = GetPropertyByName(type, propertyName);

        var errorMessage = string.Format($"{type.Name}.{propertyName} property not found!");

        Assert.IsNotNull(property, errorMessage);

        Assert.That(collectionType.IsAssignableFrom(property.PropertyType));
    }

    private static Type GetDbSetType(string modelName)
    {
        var modelType = GetModelType(modelName);

        var dbSetType = typeof(DbSet<>).MakeGenericType(modelType);
        return dbSetType;
    }

    private static Type GetModelType(string modelName)
    {
        var modelType = CurrentAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == modelName);

        Assert.IsNotNull(modelType, $"{modelName} model not found!");

        return modelType;
    }

    private class DbSetInfo
    {
        public DbSetInfo(string dbSetName, string entityName, IEnumerable<string> properties)
        {
            this.DbSetName = dbSetName;
            this.EntityName = entityName;
            this.Properties = properties;
        }

        public string DbSetName { get; }

        public string EntityName { get; }

        public IEnumerable<string> Properties { get; }
    }
}
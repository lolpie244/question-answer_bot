﻿// <auto-generated />
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace db_namespace
{
    [DbContext(typeof(dbContext))]
    public partial class dbContextModel : RuntimeModel
    {
        static dbContextModel()
        {
            var model = new dbContextModel();
            model.Initialize();
            model.Customize();
            _instance = model;
        }

        private static dbContextModel _instance;
        public static IModel Instance => _instance;

        partial void Initialize();

        partial void Customize();
    }
}

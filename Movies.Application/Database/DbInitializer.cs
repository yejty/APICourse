﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Database
{
    public class DbInitializer
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DbInitializer(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task InitializeAsync() 
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync();
            await connection.ExecuteAsync("""
                create table if not exists movies (
                id UUID primary key, 
                slug TEXT not null,
                title TEXT not null,
                yearofrelease integer not null);
             """);
            await connection.ExecuteAsync("""
                create unique index concurrently if not exists movies_slug_id on moevies using btree(slug)
             """);
        }
    }
}

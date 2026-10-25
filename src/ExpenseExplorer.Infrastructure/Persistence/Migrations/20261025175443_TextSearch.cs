using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TextSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Both extensions ship with Postgres (contrib) and are trusted, so the database owner can create them.
            migrationBuilder.Sql("""
                CREATE EXTENSION IF NOT EXISTS unaccent WITH SCHEMA public;
                CREATE EXTENSION IF NOT EXISTS pg_trgm WITH SCHEMA public;

                -- Lower case without diacritics: 'Żółty' -> 'zolty'. The dictionary is named so the function can be immutable.
                CREATE FUNCTION expense.search_key(value text) RETURNS text
                    LANGUAGE sql IMMUTABLE STRICT PARALLEL SAFE
                    RETURN lower(public.unaccent('public.unaccent'::regdictionary, value));

                CREATE FUNCTION expense.search_contains(value text, search text) RETURNS boolean
                    LANGUAGE sql IMMUTABLE STRICT PARALLEL SAFE
                    RETURN strpos(expense.search_key(value), expense.search_key(search)) > 0;

                -- 0..1: how closely the most alike stretch of words in the value resembles the search.
                CREATE FUNCTION expense.search_similarity(value text, search text) RETURNS real
                    LANGUAGE sql IMMUTABLE STRICT PARALLEL SAFE
                    RETURN public.word_similarity(expense.search_key(search), expense.search_key(value));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION expense.search_similarity(text, text);
                DROP FUNCTION expense.search_contains(text, text);
                DROP FUNCTION expense.search_key(text);
                DROP EXTENSION IF EXISTS pg_trgm;
                DROP EXTENSION IF EXISTS unaccent;
                """);
        }
    }
}

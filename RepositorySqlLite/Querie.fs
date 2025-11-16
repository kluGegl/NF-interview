module RepositorySqlLite.Query

let countTables = """SELECT COUNT(0) FROM sqlite_master where tbl_name = $table_name AND type = 'table' """
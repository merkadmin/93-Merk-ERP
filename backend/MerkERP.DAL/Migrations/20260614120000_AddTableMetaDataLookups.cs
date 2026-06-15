using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerkERP.DAL.Migrations
{
    public partial class AddTableMetaDataLookups : Migration
    {
        protected override void Up(MigrationBuilder m)
        {
            m.Sql(@"
-- 1. Create lookup tables
CREATE TABLE TableMetaData_FilterType_s (
    Id   INT           NOT NULL PRIMARY KEY,
    Name NVARCHAR(50)  NOT NULL
);
CREATE TABLE TableMetaData_DataType_s (
    Id   INT           NOT NULL PRIMARY KEY,
    Name NVARCHAR(50)  NOT NULL
);
CREATE TABLE TableMetaData_RenderAs_s (
    Id   INT           NOT NULL PRIMARY KEY,
    Name NVARCHAR(50)  NOT NULL
);

-- 2. Seed values
INSERT INTO TableMetaData_FilterType_s (Id, Name) VALUES
    (1, 'text'), (2, 'number'), (3, 'boolean'), (4, 'select');

INSERT INTO TableMetaData_DataType_s (Id, Name) VALUES
    (1, 'string'), (2, 'number'), (3, 'boolean'), (4, 'date');

INSERT INTO TableMetaData_RenderAs_s (Id, Name) VALUES
    (1, 'text'), (2, 'badge'), (3, 'yesno'), (4, 'tree');

-- 3. Register in TableName_s
INSERT INTO TableName_s (Id, Name, EntityKey) VALUES
    (26, 'TableMetaData_FilterType_s', NULL),
    (27, 'TableMetaData_DataType_s',   NULL),
    (28, 'TableMetaData_RenderAs_s',   NULL);

-- 4. Add FK columns with default = 1 (text / string / text)
ALTER TABLE TableMetaData ADD FilterTypeId INT NOT NULL DEFAULT 1;
ALTER TABLE TableMetaData ADD DataTypeId   INT NOT NULL DEFAULT 1;
ALTER TABLE TableMetaData ADD RenderAsId   INT NOT NULL DEFAULT 1;

-- 5. Populate from existing string columns
UPDATE TableMetaData SET FilterTypeId = 1 WHERE FilterType = 'text';
UPDATE TableMetaData SET FilterTypeId = 2 WHERE FilterType = 'number';
UPDATE TableMetaData SET FilterTypeId = 3 WHERE FilterType = 'boolean';
UPDATE TableMetaData SET FilterTypeId = 4 WHERE FilterType = 'select';

UPDATE TableMetaData SET DataTypeId = 1 WHERE DataType = 'string';
UPDATE TableMetaData SET DataTypeId = 2 WHERE DataType = 'number';
UPDATE TableMetaData SET DataTypeId = 3 WHERE DataType = 'boolean';
UPDATE TableMetaData SET DataTypeId = 4 WHERE DataType = 'date';

UPDATE TableMetaData SET RenderAsId = 1 WHERE RenderAs = 'text';
UPDATE TableMetaData SET RenderAsId = 2 WHERE RenderAs = 'badge';
UPDATE TableMetaData SET RenderAsId = 3 WHERE RenderAs = 'yesno';
UPDATE TableMetaData SET RenderAsId = 4 WHERE RenderAs = 'tree';

-- 6. Drop old string columns
DECLARE @sql NVARCHAR(MAX) = N'';
SELECT @sql += 'ALTER TABLE TableMetaData DROP CONSTRAINT ' + dc.name + ';'
FROM sys.default_constraints dc
JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE OBJECT_NAME(dc.parent_object_id) = 'TableMetaData'
  AND c.name IN ('FilterType', 'DataType', 'RenderAs');
EXEC sp_executesql @sql;

ALTER TABLE TableMetaData DROP COLUMN FilterType;
ALTER TABLE TableMetaData DROP COLUMN DataType;
ALTER TABLE TableMetaData DROP COLUMN RenderAs;

-- 7. Add FK constraints
ALTER TABLE TableMetaData ADD CONSTRAINT FK_TableMetaData_FilterType
    FOREIGN KEY (FilterTypeId) REFERENCES TableMetaData_FilterType_s(Id);
ALTER TABLE TableMetaData ADD CONSTRAINT FK_TableMetaData_DataType
    FOREIGN KEY (DataTypeId)   REFERENCES TableMetaData_DataType_s(Id);
ALTER TABLE TableMetaData ADD CONSTRAINT FK_TableMetaData_RenderAs
    FOREIGN KEY (RenderAsId)   REFERENCES TableMetaData_RenderAs_s(Id);

-- 8. Update stored procedure to JOIN with lookup tables
EXEC(N'
CREATE OR ALTER PROCEDURE sp_GetTableMetaData
    @TableNameId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        tm.Id,
        tm.TableNameId,
        tn.Name             AS TableName,
        tn.EntityKey,
        tm.[Key],
        tm.LabelEN,
        tm.LabelAR,
        tm.ColumnOrder,
        tm.EntityProperty,
        tm.ForeignKeyProperty,
        ft.Name             AS FilterType,
        dt.Name             AS DataType,
        ra.Name             AS RenderAs,
        tm.IsSortable,
        tm.IsFilterable,
        tm.IsVisible,
        tm.MinWidth
    FROM TableMetaData tm
    INNER JOIN TableName_s tn               ON tn.Id = tm.TableNameId
    INNER JOIN TableMetaData_FilterType_s ft ON ft.Id = tm.FilterTypeId
    INNER JOIN TableMetaData_DataType_s   dt ON dt.Id = tm.DataTypeId
    INNER JOIN TableMetaData_RenderAs_s   ra ON ra.Id = tm.RenderAsId
    WHERE tm.TableNameId = @TableNameId
    ORDER BY tm.ColumnOrder;
END
');
");
        }

        protected override void Down(MigrationBuilder m)
        {
            m.Sql(@"
ALTER TABLE TableMetaData DROP CONSTRAINT FK_TableMetaData_FilterType;
ALTER TABLE TableMetaData DROP CONSTRAINT FK_TableMetaData_DataType;
ALTER TABLE TableMetaData DROP CONSTRAINT FK_TableMetaData_RenderAs;

ALTER TABLE TableMetaData ADD FilterType NVARCHAR(20) NOT NULL DEFAULT 'text';
ALTER TABLE TableMetaData ADD DataType   NVARCHAR(20) NOT NULL DEFAULT 'string';
ALTER TABLE TableMetaData ADD RenderAs   NVARCHAR(20) NOT NULL DEFAULT 'text';

UPDATE TableMetaData SET FilterType = ft.Name FROM TableMetaData tm JOIN TableMetaData_FilterType_s ft ON ft.Id = tm.FilterTypeId;
UPDATE TableMetaData SET DataType   = dt.Name FROM TableMetaData tm JOIN TableMetaData_DataType_s   dt ON dt.Id = tm.DataTypeId;
UPDATE TableMetaData SET RenderAs   = ra.Name FROM TableMetaData tm JOIN TableMetaData_RenderAs_s   ra ON ra.Id = tm.RenderAsId;

ALTER TABLE TableMetaData DROP COLUMN FilterTypeId;
ALTER TABLE TableMetaData DROP COLUMN DataTypeId;
ALTER TABLE TableMetaData DROP COLUMN RenderAsId;

DROP TABLE TableMetaData_FilterType_s;
DROP TABLE TableMetaData_DataType_s;
DROP TABLE TableMetaData_RenderAs_s;

DELETE FROM TableName_s WHERE Id IN (26, 27, 28);
");
        }
    }
}

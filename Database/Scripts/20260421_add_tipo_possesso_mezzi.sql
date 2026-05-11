IF COL_LENGTH('dbo.Mezzi', 'TipoPossesso') IS NULL
BEGIN
    ALTER TABLE dbo.Mezzi
    ADD TipoPossesso NVARCHAR(20) NULL;
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_Mezzi_TipoPossesso'
)
BEGIN
    ALTER TABLE dbo.Mezzi
    ADD CONSTRAINT CK_Mezzi_TipoPossesso
        CHECK (TipoPossesso IS NULL OR TipoPossesso IN ('noleggio', 'proprieta'));
END;
GO

/****** Object:  StoredProcedure [GetD11_City_ReChild] ******/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[GetD11_City_ReChild]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [GetD11_City_ReChild]
GO

CREATE PROCEDURE [GetD11_City_ReChild]
    @City_ID2 int
AS
    BEGIN

        SET NOCOUNT ON

        /* Get D11_City_ReChild from table */
        SELECT
            [5_Cities_ReChild].[City_Child_Name]
        FROM [5_Cities_ReChild]
        WHERE
            [5_Cities_ReChild].[City_ID2] = @City_ID2

    END
GO

/****** Object:  StoredProcedure [AddD11_City_ReChild] ******/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[AddD11_City_ReChild]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [AddD11_City_ReChild]
GO

CREATE PROCEDURE [AddD11_City_ReChild]
    @City_ID2 int,
    @City_Child_Name varchar(50)
AS
    BEGIN

        SET NOCOUNT ON

        /* Insert object into 5_Cities_ReChild */
        INSERT INTO [5_Cities_ReChild]
        (
            [City_ID2],
            [City_Child_Name]
        )
        VALUES
        (
            @City_ID2,
            @City_Child_Name
        )

    END
GO

/****** Object:  StoredProcedure [UpdateD11_City_ReChild] ******/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[UpdateD11_City_ReChild]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [UpdateD11_City_ReChild]
GO

CREATE PROCEDURE [UpdateD11_City_ReChild]
    @City_ID2 int,
    @City_Child_Name varchar(50)
AS
    BEGIN

        SET NOCOUNT ON

        /* Check for object existence */
        IF NOT EXISTS
        (
            SELECT [City_ID2] FROM [5_Cities_ReChild]
            WHERE
                [City_ID2] = @City_ID2
        )
        BEGIN
            RAISERROR ('''D11_City_ReChild'' object not found. It was probably removed by another user.', 16, 1)
            RETURN
        END

        /* Update object in 5_Cities_ReChild */
        UPDATE [5_Cities_ReChild]
        SET
            [City_Child_Name] = @City_Child_Name
        WHERE
            [City_ID2] = @City_ID2

    END
GO

/****** Object:  StoredProcedure [DeleteD11_City_ReChild] ******/
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DeleteD11_City_ReChild]') AND type in (N'P', N'PC'))
    DROP PROCEDURE [DeleteD11_City_ReChild]
GO

CREATE PROCEDURE [DeleteD11_City_ReChild]
    @City_ID2 int
AS
    BEGIN

        SET NOCOUNT ON

        /* Check for object existence */
        IF NOT EXISTS
        (
            SELECT [City_ID2] FROM [5_Cities_ReChild]
            WHERE
                [City_ID2] = @City_ID2
        )
        BEGIN
            RAISERROR ('''D11_City_ReChild'' object not found. It was probably removed by another user.', 16, 1)
            RETURN
        END

        /* Delete D11_City_ReChild object from 5_Cities_ReChild */
        DELETE
        FROM [5_Cities_ReChild]
        WHERE
            [5_Cities_ReChild].[City_ID2] = @City_ID2

    END
GO

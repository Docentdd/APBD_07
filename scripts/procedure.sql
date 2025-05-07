CREATE PROCEDURE AddDevice
    @Id NVARCHAR(50),
    @Name NVARCHAR(100),
    @IsEnabled BIT,
    @DeviceType NVARCHAR(50),
    @BatteryPercentage INT = NULL,
    @OperatingSystem NVARCHAR(100) = NULL,
    @IpAddress NVARCHAR(50) = NULL,
    @NetworkName NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

BEGIN TRY
BEGIN TRANSACTION;

        -- Insert into the base Device table
INSERT INTO Device (Id, Name, IsEnabled, DeviceType)
VALUES (@Id, @Name, @IsEnabled, @DeviceType);

-- Insert into specific device type tables
IF @DeviceType = 'Smartwatch'
BEGIN
INSERT INTO Smartwatch (DeviceId, BatteryPercentage)
VALUES (@Id, @BatteryPercentage);
END
ELSE IF @DeviceType = 'PersonalComputer'
BEGIN
INSERT INTO PersonalComputer (DeviceId, OperatingSystem)
VALUES (@Id, @OperatingSystem);
END
ELSE IF @DeviceType = 'Embedded'
BEGIN
INSERT INTO Embedded (DeviceId, IpAddress, NetworkName)
VALUES (@Id, @IpAddress, @NetworkName);
END

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
ROLLBACK TRANSACTION;
        THROW;
END CATCH
END;
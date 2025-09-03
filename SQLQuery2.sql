use FractoDB

select * from Appointments
select * from Users
select * from Doctors
select * from Specializations
-- Check if the table is empty before inserting to avoid duplicates
IF NOT EXISTS (SELECT 1 FROM dbo.Specializations)
BEGIN
    -- Insert some sample specializations into the Specializations table
    INSERT INTO dbo.Specializations (specializationName)
    VALUES
        ('Cardiology'),       -- This will have specializationId = 1
        ('Dermatology'),      -- This will have specializationId = 2
        ('Orthopedics'),      -- This will have specializationId = 3
        ('Neurology'),        -- This will have specializationId = 4
        ('Pediatrics');       -- This will have specializationId = 5
END
GO
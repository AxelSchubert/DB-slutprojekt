CREATE PROCEDURE DisplayStudentInfo
    @StudentID INT
AS
BEGIN
    SELECT StudentID, FullName, SocialSecurityNumber, ContactInfo    
	FROM Students
    WHERE StudentID = @StudentID;
END;
--Ett exempel p� studenten med id 3
EXEC DisplayStudentInfo @StudentID = 3

GO
--Transaction som s�tter ett betyg utifr�n Student- och kursID i StudentCourse
CREATE PROCEDURE SetGrade
    @StudentID INT,
    @CourseID INT,
    @Grade NVARCHAR(1)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE StudentCourse
        SET Grade = @Grade, GradeDate = GETDATE()
        WHERE StudentID_FK = @StudentID AND CourseID_FK = @CourseID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;

DROP PROCEDURE SetGrade

--Exempel p� SetGrade
EXEC SetGrade @StudentID = 3, @CourseID = 2, @Grade = B

SELECT * FROM StudentCourse

CREATE PROCEDURE DisplayStudentInfo
    @StudentID INT
AS
BEGIN
    SELECT *    
	FROM Students
    WHERE StudentID = @StudentID;
END;
--Ett exempel på studenten med id 3
EXEC DisplayStudentInfo @StudentID = 3

GO
--Transaction som sätter ett betyg utifrån Student- och kursID i StudentCourse
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

--Exempel på SetGrade
EXEC SetGrade @StudentID = 3, @CourseID = 2, @Grade = B

SELECT * FROM StudentCourse

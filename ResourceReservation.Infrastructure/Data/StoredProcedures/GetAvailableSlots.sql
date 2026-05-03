CREATE OR ALTER PROCEDURE [dbo].[GetAvailableSlotsForWeek]
    @ResourceId     INT,
    @FromDate       DATE,
    @ToDate         DATE,
    @WorkdayStart   TIME = '08:00',
    @WorkdayEnd     TIME = '18:00'
AS
BEGIN
    SET NOCOUNT ON;

    -- جدول موقت برای تمام روزهای بازه
    DECLARE @Days TABLE (DayDate DATE);
    DECLARE @Current DATE = @FromDate;
    WHILE @Current <= @ToDate
BEGIN
INSERT INTO @Days VALUES (@Current);
SET @Current = DATEADD(DAY, 1, @Current);
END

    -- رزروهای فعال در این بازه
    ;WITH BusySlots AS (
    SELECT
        CAST(StartTime AS DATE)  AS ReservationDate,
        CAST(StartTime AS TIME)  AS BusyStart,
        CAST(EndTime   AS TIME)  AS BusyEnd
    FROM Reservations
    WHERE ResourceId = @ResourceId
      AND Status     = 1   -- Active
      AND StartTime  < CAST(@ToDate   AS DATETIME2) + 1
      AND EndTime    > CAST(@FromDate AS DATETIME2)
),
          -- محاسبه شکاف‌های خالی روی هر روز با LAG
          DailyGaps AS (
              SELECT
                  d.DayDate,
                  @WorkdayStart AS GapStart,
                  MIN(bs.BusyStart) AS GapEnd
              FROM @Days d
                       LEFT JOIN BusySlots bs ON bs.ReservationDate = d.DayDate
              GROUP BY d.DayDate

              UNION ALL

              SELECT
                  d.DayDate,
                  MAX(bs.BusyEnd)  AS GapStart,
                  @WorkdayEnd      AS GapEnd
              FROM @Days d
                       LEFT JOIN BusySlots bs ON bs.ReservationDate = d.DayDate
              GROUP BY d.DayDate
          )
     SELECT
         CAST(CAST(DayDate AS VARCHAR) + ' ' + CAST(GapStart AS VARCHAR) AS DATETIME2) AS StartTime,
         CAST(CAST(DayDate AS VARCHAR) + ' ' + CAST(GapEnd   AS VARCHAR) AS DATETIME2) AS EndTime
     FROM DailyGaps
     WHERE GapStart < GapEnd
     ORDER BY StartTime;
END
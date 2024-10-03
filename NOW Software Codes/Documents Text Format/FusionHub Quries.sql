

SELECT 
    op.operator_id, 
    op.name, 
    op.logo,
    op.short_code,
	op.custom_image,
    CASE 
        WHEN op.custom_image IS NULL THEN op.logo 
        ELSE op.custom_image 
    END AS myImageURL
FROM dbo.operator op
WHERE op.short_code = 'amazon'









DECLARE @total_pages INT,
        @total_records INT,
		@operatorFilter operator_filters_type;


		INSERT @operatorFilter
		(
		    operator_name,
		    country_iso_code
		)
		VALUES
		(   NULL, -- operator_name - nvarchar(50)
		    'us' -- country_iso_code - nvarchar(2)
		    )


EXEC dbo.operatorTest @OperatorFilters = @operatorFilter,               -- operator_filters_type
                         @is_active = 1,                     -- bit
                         @is_deleted = 0,                    -- bit
                         @page = 1,                             -- int
                         @records_per_page = 100,                 -- int
                         @total_pages = @total_pages OUTPUT,    -- int
                         @total_records = @total_records OUTPUT -- int







SELECT  op.operator_id, op.name, op.short_code, op.name_alias , op.custom_image
FROM dbo.operator op




                         

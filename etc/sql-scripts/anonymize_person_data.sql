BEGIN;

UPDATE data.persons
SET birth_year = 1940 + floor(random() * 66)::integer;

WITH eligible_cantons AS (
    SELECT array_agg(id ORDER BY id) AS ids
    FROM data.cantons
    WHERE NOT is_deleted
)
UPDATE data.addresses
SET street = 'Bahnhofstrasse 1337',
    zip = '3000',
    city = 'Bern',
    canton_id = eligible_cantons.ids[
        1 + floor(random() * cardinality(eligible_cantons.ids))::integer
    ]
FROM eligible_cantons;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM data.persons
        WHERE birth_year NOT BETWEEN 1940 AND 2005
    ) THEN
        RAISE EXCEPTION 'Person anonymization validation failed: invalid birth year';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM data.addresses AS addresses
        LEFT JOIN data.cantons AS cantons
            ON cantons.id = addresses.canton_id
            AND NOT cantons.is_deleted
        WHERE addresses.street IS DISTINCT FROM 'Bahnhofstrasse 1337'
            OR addresses.zip IS DISTINCT FROM '3000'
            OR addresses.city IS DISTINCT FROM 'Bern'
            OR cantons.id IS NULL
    ) THEN
        RAISE EXCEPTION 'Address anonymization validation failed';
    END IF;
END
$$;

COMMIT;

-- APG ODS Views Creation Script
-- This script creates all Open Data Service (ODS) views for the APG system
-- Set grant_user in the DO block at the end to the database user to grant permissions to

-- View 1: ods_addresses
CREATE OR REPLACE VIEW data.ods_addresses AS SELECT * FROM data.addresses;

-- View 2: ods_appointment_decision_link_types
CREATE OR REPLACE VIEW data.ods_appointment_decision_link_types AS SELECT * FROM data.appointment_decision_link_types;

-- View 3: ods_appointment_decision_types
CREATE OR REPLACE VIEW data.ods_appointment_decision_types AS SELECT * FROM data.appointment_decision_types;

-- View 4: ods_appointment_decisions
CREATE OR REPLACE VIEW data.ods_appointment_decisions AS SELECT * FROM data.appointment_decisions;

-- View 5: ods_candidate_list_states
CREATE OR REPLACE VIEW data.ods_candidate_list_states AS SELECT * FROM data.candidate_list_states;

-- View 6: ods_cantons
CREATE OR REPLACE VIEW data.ods_cantons AS SELECT * FROM data.cantons;

-- View 7: ods_committee_membership_addition
CREATE OR REPLACE VIEW data.ods_committee_membership_addition AS SELECT * FROM data.committee_membership_addition;

-- View 8: ods_committees
CREATE OR REPLACE VIEW data.ods_committees AS SELECT * FROM data.committees;

-- View 9: ods_committees_levels
CREATE OR REPLACE VIEW data.ods_committees_levels AS SELECT * FROM data.committee_levels;

-- View 10: ods_committees_types
CREATE OR REPLACE VIEW data.ods_committees_types AS SELECT * FROM data.committee_types;

-- View 11: ods_contact_point_types
CREATE OR REPLACE VIEW data.ods_contact_point_types AS SELECT * FROM data.contact_point_types;

-- View 12: ods_contact_points
CREATE OR REPLACE VIEW data.ods_contact_points AS SELECT * FROM data.contact_points;

-- View 13: ods_councils
CREATE OR REPLACE VIEW data.ods_councils AS SELECT * FROM data.councils;

-- View 14: ods_departments
CREATE OR REPLACE VIEW data.ods_departments AS SELECT * FROM data.departments;

-- View 15: ods_eiam_assignments
CREATE OR REPLACE VIEW data.ods_eiam_assignments AS SELECT * FROM data.eiam_assignments;

-- View 16: ods_election_offices
CREATE OR REPLACE VIEW data.ods_election_offices AS SELECT * FROM data.election_offices;

-- View 17: ods_election_types
CREATE OR REPLACE VIEW data.ods_election_types AS SELECT * FROM data.election_types;

-- View 18: ods_functions
CREATE OR REPLACE VIEW data.ods_functions AS SELECT * FROM data.functions;

-- View 19: ods_genders
CREATE OR REPLACE VIEW data.ods_genders AS SELECT * FROM data.genders;

-- View 20: ods_general_election_committees
CREATE OR REPLACE VIEW data.ods_general_election_committees AS SELECT * FROM data.general_election_committees;

-- View 21: ods_interest_committees
CREATE OR REPLACE VIEW data.ods_interest_committees AS SELECT * FROM data.interest_committees;

-- View 22: ods_interest_functions
CREATE OR REPLACE VIEW data.ods_interest_functions AS SELECT * FROM data.interest_functions;

-- View 23: ods_interests
CREATE OR REPLACE VIEW data.ods_interests AS SELECT * FROM data.interests;

-- View 24: ods_languages
CREATE OR REPLACE VIEW data.ods_languages AS SELECT * FROM data.languages;

-- View 25: ods_legal_forms
CREATE OR REPLACE VIEW data.ods_legal_forms AS SELECT * FROM data.legal_forms;

-- View 26: ods_legislature_periods
CREATE OR REPLACE VIEW data.ods_legislature_periods AS SELECT * FROM data.legislature_periods;

-- View 27: ods_membership_additions
CREATE OR REPLACE VIEW data.ods_membership_additions AS SELECT * FROM data.membership_additions;

-- View 28: ods_membership_candidates
CREATE OR REPLACE VIEW data.ods_membership_candidates AS SELECT * FROM data.membership_candidates;

-- View 29: ods_memberships
CREATE OR REPLACE VIEW data.ods_memberships AS SELECT * FROM data.memberships;

-- View 30: ods_occupations
CREATE OR REPLACE VIEW data.ods_occupations AS SELECT * FROM data.occupations;

-- View 31: ods_offices
CREATE OR REPLACE VIEW data.ods_offices AS SELECT * FROM data.offices;

-- View 32: ods_person_legislature_period
CREATE OR REPLACE VIEW data.ods_person_legislature_period AS SELECT * FROM data.person_legislature_period;

-- View 33: ods_person_occupations
CREATE OR REPLACE VIEW data.ods_person_occupations AS SELECT * FROM data.person_occupations;

-- View 34: ods_persons
CREATE OR REPLACE VIEW data.ods_persons AS SELECT * FROM data.persons;

-- View 35: ods_term_of_office_dates
CREATE OR REPLACE VIEW data.ods_term_of_office_dates AS SELECT * FROM data.term_of_office_dates;

-- View 36: ods_terms_of_office
CREATE OR REPLACE VIEW data.ods_terms_of_office AS SELECT * FROM data.terms_of_office;

-- View 37: ods_worklist_task_states
CREATE OR REPLACE VIEW data.ods_worklist_task_states AS SELECT * FROM data.worklist_task_states;

-- View 38: ods_worklist_task_types
CREATE OR REPLACE VIEW data.ods_worklist_task_types AS SELECT * FROM data.worklist_task_types;

-- View 39: ods_worklist_tasks
CREATE OR REPLACE VIEW data.ods_worklist_tasks AS SELECT * FROM data.worklist_tasks;

-- Grant SELECT on all ODS views to the given user.
-- PostgreSQL has no script-level variables, so an anonymous code block (DO) is used instead.
DO $$
DECLARE
    -- Database user that receives SELECT permission on the ODS views
    grant_user TEXT := 'your_user_name';
    -- Holds one row of the query result per loop iteration (here: the view name)
    v RECORD;
BEGIN
    -- Iterate over all views in schema "data" whose name starts with "ods_"
    -- ("\_" escapes the underscore, which would otherwise match any single character)
    FOR v IN SELECT table_name FROM information_schema.views
             WHERE table_schema = 'data' AND table_name LIKE 'ods\_%'
    LOOP
        -- GRANT does not accept variables, so the statement is built dynamically.
        -- %I quotes the value safely as an identifier (view and user name).
        EXECUTE format('GRANT SELECT ON data.%I TO %I', v.table_name, grant_user);
    END LOOP;
END $$;

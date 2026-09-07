-- APG ODS Views Creation Script
-- This script creates all Open Data Service (ODS) views for the APG system
-- The @grantUser variable represents the database user to grant permissions to

DECLARE @grantUser NVARCHAR(MAX) = 'your_user_name'

-- View 1: ods_addresses
CREATE OR REPLACE VIEW data.ods_addresses AS SELECT * FROM data.addresses
GRANT SELECT ON data.ods_addresses TO @grantUser

-- View 2: ods_appointment_decision_link_types
CREATE OR REPLACE VIEW data.ods_appointment_decision_link_types AS SELECT * FROM data.appointment_decision_link_types
GRANT SELECT ON data.ods_appointment_decision_link_types TO @grantUser

-- View 3: ods_appointment_decision_types
CREATE OR REPLACE VIEW data.ods_appointment_decision_types AS SELECT * FROM data.appointment_decision_types
GRANT SELECT ON data.ods_appointment_decision_types TO @grantUser

-- View 4: ods_appointment_decisions
CREATE OR REPLACE VIEW data.ods_appointment_decisions AS SELECT * FROM data.appointment_decisions
GRANT SELECT ON data.ods_appointment_decisions TO @grantUser

-- View 5: ods_candidate_list_states
CREATE OR REPLACE VIEW data.ods_candidate_list_states AS SELECT * FROM data.candidate_list_states
GRANT SELECT ON data.ods_candidate_list_states TO @grantUser

-- View 6: ods_cantons
CREATE OR REPLACE VIEW data.ods_cantons AS SELECT * FROM data.cantons
GRANT SELECT ON data.ods_cantons TO @grantUser

-- View 7: ods_committee_membership_addition
CREATE OR REPLACE VIEW data.ods_committee_membership_addition AS SELECT * FROM data.committee_membership_addition
GRANT SELECT ON data.ods_committee_membership_addition TO @grantUser

-- View 8: ods_committees
CREATE OR REPLACE VIEW data.ods_committees AS SELECT * FROM data.committees
GRANT SELECT ON data.ods_committees TO @grantUser

-- View 9: ods_committees_levels
CREATE OR REPLACE VIEW data.ods_committees_levels AS SELECT * FROM data.committee_levels
GRANT SELECT ON data.ods_committees_levels TO @grantUser

-- View 10: ods_committees_types
CREATE OR REPLACE VIEW data.ods_committees_types AS SELECT * FROM data.committee_types
GRANT SELECT ON data.ods_committees_types TO @grantUser

-- View 11: ods_contact_point_types
CREATE OR REPLACE VIEW data.ods_contact_point_types AS SELECT * FROM data.contact_point_types
GRANT SELECT ON data.ods_contact_point_types TO @grantUser

-- View 12: ods_contact_points
CREATE OR REPLACE VIEW data.ods_contact_points AS SELECT * FROM data.contact_points
GRANT SELECT ON data.ods_contact_points TO @grantUser

-- View 13: ods_councils
CREATE OR REPLACE VIEW data.ods_councils AS SELECT * FROM data.councils
GRANT SELECT ON data.ods_councils TO @grantUser

-- View 14: ods_departments
CREATE OR REPLACE VIEW data.ods_departments AS SELECT * FROM data.departments
GRANT SELECT ON data.ods_departments TO @grantUser

-- View 15: ods_eiam_assignments
CREATE OR REPLACE VIEW data.ods_eiam_assignments AS SELECT * FROM data.eiam_assignments
GRANT SELECT ON data.ods_eiam_assignments TO @grantUser

-- View 16: ods_election_offices
CREATE OR REPLACE VIEW data.ods_election_offices AS SELECT * FROM data.election_offices
GRANT SELECT ON data.ods_election_offices TO @grantUser

-- View 17: ods_election_types
CREATE OR REPLACE VIEW data.ods_election_types AS SELECT * FROM data.election_types
GRANT SELECT ON data.ods_election_types TO @grantUser

-- View 18: ods_functions
CREATE OR REPLACE VIEW data.ods_functions AS SELECT * FROM data.functions
GRANT SELECT ON data.ods_functions TO @grantUser

-- View 19: ods_genders
CREATE OR REPLACE VIEW data.ods_genders AS SELECT * FROM data.genders
GRANT SELECT ON data.ods_genders TO @grantUser

-- View 20: ods_general_election_committees
CREATE OR REPLACE VIEW data.ods_general_election_committees AS SELECT * FROM data.general_election_committees
GRANT SELECT ON data.ods_general_election_committees TO @grantUser

-- View 21: ods_interest_committees
CREATE OR REPLACE VIEW data.ods_interest_committees AS SELECT * FROM data.interest_committees
GRANT SELECT ON data.ods_interest_committees TO @grantUser

-- View 22: ods_interest_functions
CREATE OR REPLACE VIEW data.ods_interest_functions AS SELECT * FROM data.interest_functions
GRANT SELECT ON data.ods_interest_functions TO @grantUser

-- View 23: ods_interests
CREATE OR REPLACE VIEW data.ods_interests AS SELECT * FROM data.interests
GRANT SELECT ON data.ods_interests TO @grantUser

-- View 24: ods_languages
CREATE OR REPLACE VIEW data.ods_languages AS SELECT * FROM data.languages
GRANT SELECT ON data.ods_languages TO @grantUser

-- View 25: ods_legal_forms
CREATE OR REPLACE VIEW data.ods_legal_forms AS SELECT * FROM data.legal_forms
GRANT SELECT ON data.ods_legal_forms TO @grantUser

-- View 26: ods_legislature_periods
CREATE OR REPLACE VIEW data.ods_legislature_periods AS SELECT * FROM data.legislature_periods
GRANT SELECT ON data.ods_legislature_periods TO @grantUser

-- View 27: ods_membership_additions
CREATE OR REPLACE VIEW data.ods_membership_additions AS SELECT * FROM data.membership_additions
GRANT SELECT ON data.ods_membership_additions TO @grantUser

-- View 28: ods_membership_candidates
CREATE OR REPLACE VIEW data.ods_membership_candidates AS SELECT * FROM data.membership_candidates
GRANT SELECT ON data.ods_membership_candidates TO @grantUser

-- View 29: ods_memberships
CREATE OR REPLACE VIEW data.ods_memberships AS SELECT * FROM data.memberships
GRANT SELECT ON data.ods_memberships TO @grantUser

-- View 30: ods_occupations
CREATE OR REPLACE VIEW data.ods_occupations AS SELECT * FROM data.occupations
GRANT SELECT ON data.ods_occupations TO @grantUser

-- View 31: ods_offices
CREATE OR REPLACE VIEW data.ods_offices AS SELECT * FROM data.offices
GRANT SELECT ON data.ods_offices TO @grantUser

-- View 32: ods_person_legislature_period
CREATE OR REPLACE VIEW data.ods_person_legislature_period AS SELECT * FROM data.person_legislature_period
GRANT SELECT ON data.ods_person_legislature_period TO @grantUser

-- View 33: ods_person_occupations
CREATE OR REPLACE VIEW data.ods_person_occupations AS SELECT * FROM data.person_occupations
GRANT SELECT ON data.ods_person_occupations TO @grantUser

-- View 34: ods_persons
CREATE OR REPLACE VIEW data.ods_persons AS SELECT * FROM data.persons
GRANT SELECT ON data.ods_persons TO @grantUser

-- View 35: ods_term_of_office_dates
CREATE OR REPLACE VIEW data.ods_term_of_office_dates AS SELECT * FROM data.term_of_office_dates
GRANT SELECT ON data.ods_term_of_office_dates TO @grantUser

-- View 36: ods_terms_of_office
CREATE OR REPLACE VIEW data.ods_terms_of_office AS SELECT * FROM data.terms_of_office
GRANT SELECT ON data.ods_terms_of_office TO @grantUser

-- View 37: ods_worklist_task_states
CREATE OR REPLACE VIEW data.ods_worklist_task_states AS SELECT * FROM data.worklist_task_states
GRANT SELECT ON data.ods_worklist_task_states TO @grantUser

-- View 38: ods_worklist_task_types
CREATE OR REPLACE VIEW data.ods_worklist_task_types AS SELECT * FROM data.worklist_task_types
GRANT SELECT ON data.ods_worklist_task_types TO @grantUser

-- View 39: ods_worklist_tasks
CREATE OR REPLACE VIEW data.ods_worklist_tasks AS SELECT * FROM data.worklist_tasks
GRANT SELECT ON data.ods_worklist_tasks TO @grantUser

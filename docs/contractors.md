# Contractors and job assignments

Use **Contractors** in the sidebar to add or edit contractor records. Each record
has a name, optional company, phone, email and notes, plus an active switch.
Use **View jobs** to open the jobs list filtered to that contractor.

The job form assigns work to contractors. New jobs start unassigned. Active
contractors can receive new assignments; inactive contractors stay visible on
their existing jobs and can be replaced or cleared. Jobs list filters, mobile
cards and job-sheet PDFs use contractor names.

Apply the `AddContractorsAndJobAssignments` migration before using the feature:

```powershell
dotnet ef database update --project CRM.Infrastructure --startup-project CRM.Web --context CRMDbContext
```

The migration renames the existing assignment column and creates one contractor
record for each previously assigned user, carrying across the name, email, phone
and active status. It preserves the assignment IDs and imports a fallback record
if the old user no longer exists. Imported contractors can be edited independently
of user accounts. User accounts still handle sign-in and activity assignments.

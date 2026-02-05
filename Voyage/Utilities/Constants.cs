namespace Voyage.Utilities
{
    public static class Constants
    {

        #region Config

        public static void Initialize(IConfiguration config)
        {
            BaseUrl = config["AppSettings:BaseUrl"];
            DbConnString = config["AppSettings:DefaultConnection"];
        }

        public static string? BaseUrl { get; private set; }
        public static string? DbConnString { get; private set; }

        public const string SystemEmail = "mitchellfrehr@gmail.com";

        #endregion

        #region Error Messages

        //Http Responses
        public const string BadRequest = "(400) Bad Request.";

        //Database
        public const string ErrDBSaveFailed = "Save failed. Could not add user to the database.";
        public const string ErrDBGetFailed = "Could not retrieve data.";

        //Registration
        public const string ErrUsernameIsReq = "Username is required.";
        public const string ErrUsernameMinLen = "Username must be 5 or more characters.";
        public const string ErrUsernameMaxLen = "Username must not be over 20 characters.";
        public const string ErrInvalidEmail = "Valid email is required.";
        public const string ErrPassMinLen = "Password must be at least 8 characters.";
        public const string ErrPassMaxLen = "Password must be 20 characters or less.";
        public const string ErrPhoneAreaCodeIsReq = "Phone area code is required.";
        public const string ErrPhoneAreaCodeMaxLen = "Phone area code must be no longer than 5 characters.";
        public const string ErrPhoneNumReq = "Phone number is required.";
        public const string ErrPhoneNumMaxLen = "Phone number must be no longer than 10 digits.";
        public const string ErrPhoneNumMinLen = "Phone number must be at least 3 digits.";
        public const string ErrStreetAddrReq = "Street address is required.";
        public const string ErrCityReq = "City is required.";
        public const string ErrStateReq = "State is required.";
        public const string ErrCountryReq = "Country is required.";
        public const string ErrPostalCode = "Postal code is required.";
        public const string ErrPostCodeMinLen = "Postal code must be at least 3 digits.";
        public const string ErrPostCodeMaxLen = "Postal code must be 10 digits or less.";
        public const string ErrUsernameExists = "Username is taken.";
        public const string ErrFirstNameReq = "First Name is required.";
        public const string ErrLastNameReq = "Last Name is required.";

        //Login
        public const string ErrBadUsernameOrPassword = "Invalid username or password.";
        public const string TooManyLoginAttempts = "Account is locked.";
        public const string InvalidLoginAttempt = "Invalid login attempt.";

        #endregion

        #region Success Messages

        //Database
        public const string DBSaveSuccessful = "Data saved succesfully.";
        public const string DBGetSuccessful = "Data retreived successfully.";

        //Login
        public const string LoginSuccessful = "Login successful.";

        //Registration
        public const string RegistrationSuccessful = "Registration sucessful.";

        #endregion



        #region Validation Values

        public const int UsernameMinLen = 5;
        public const int UsernameMaxLen = 20;
        public const int PasswordMinLen = 8;
        public const int PasswordMaxLen = 20;
        public const int PhoneAreaCodeMaxLen = 5;
        public const int PhoneNumMaxLen = 10;
        public const int PhoneNumMinLen = 3;
        public const int PostCodeMinLen = 3;
        public const int PostCodeMaxLen = 10;


        #endregion


        #region enums
        public enum LogSeverity
        {
            Low,
            Medium,
            High
        }

        public enum LogType
        {
            Error,
            Debug,
            System
        }

        public enum SaveAction
        {
            Save = 1, //add, update if exists
            Remove = 2,
        }

        public enum GridControlType
        {
            Basic = 0,
            AllUsers = 1,
            AllTeams = 2,
            UnassignedDeptTeams = 3,
            UnassignedDeptUsers = 4,
        }

        public enum DefaultRoles
        {
            Principal = -1,
            Unassigned = -2
        }


        public enum PriorityLevel
        {
            Low,
            Medium,
            High
        }

        public enum TicketStatus
        {
            NotStarted,
            InProgress,
            Completed,
            Discontinued
        }

        public enum TicketChangeAction
        {
            CreatedTicket,
            SectionChanged,
            StatusChanged,
            TitleChanged,
            DescriptionChanged,
            Assigned,
            PriorityLevelChanged,
            DueDateChanged,
            ParentTicketChanged
        }

        public enum RepeatSprint { 
            Never,
            Weekly,
            BiWeekly,
            Monthly,
            Custom
        }

        public enum Feature
        {
            Tickets
        }

        public enum SectionSettings
        {
            Custom,
            Development
        }

        public enum RequiredTicketSections
        {
            Completed,
            Discontinued,
            Backlog
        }


        #endregion






    }


}

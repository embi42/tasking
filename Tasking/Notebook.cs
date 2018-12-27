using System;
// ReSharper disable UnusedMember.Global

namespace Tasking
{

    public class Context<T> where T : class
    {
        public string ODataContext { get; set; }
        public T[] Value { get; set; }
    }

    public class Notebook
    {
        public string Id { get; set; }
        public string Self { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public bool IsDefault { get; set; }
        public string UserRole { get; set; }
        public bool IsShared { get; set; }
        public string SectionsUrl { get; set; }
        public string SectionGroupsUrl { get; set; }
        public CreatedBy CreatedBy { get; set; }
        public LastModifiedBy LastModifiedBy { get; set; }
        public Links Links { get; set; }
    }

    public class Section
    {
        public string Id { get; set; }
        public string Self { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string DisplayName { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public bool IsDefault { get; set; }
        public string PagesUrl { get; set; }
        public CreatedBy CreatedBy { get; set; }
        public LastModifiedBy LastModifiedBy { get; set; }
        public string ParentNotebookODataContext { get; set; }
        public Parent ParentNotebook { get; set; }
        public string ParentSectionGroupODataContext { get; set; }
        public object ParentSectionGroup { get; set; }
    }

    public class Page
    {
        public string Id { get; set; }
        public string Self { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string Title { get; set; }
        public string CreatedByAppId { get; set; }
        public string ContentUrl { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public Links Links { get; set; }
        public string ParentSectionODataContext { get; set; }
        public Parent ParentSection { get; set; }
    }

    public class Parent
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Self { get; set; }
    }

    public class CreatedBy
    {
        public User User { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class LastModifiedBy
    {
        public User User { get; set; }
    }

    public class Links
    {
        public Url OneNoteClientUrl { get; set; }
        public Url OneNoteWebUrl { get; set; }
    }

    public class Url
    {
        public string Href { get; set; }
    }

    public class Change
    {
        public string target { get; set; }
        public string action { get; set; }
        public string position { get; set; }
        public string content { get; set; }
    }
}

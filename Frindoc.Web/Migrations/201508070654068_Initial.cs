namespace Frindoc.Web.Models
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Benchmarks",
                c => new
                    {
                        BenchmarkID = c.Int(nullable: false, identity: true),
                        Choice = c.Int(nullable: false),
                        Clarification = c.String(),
                        Alternative = c.String(),
                        User = c.String(),
                        ResponsiblePersonName = c.String(),
                        InstituteName = c.String(),
                        Country = c.String(),
                        NrOfUndergraduates = c.String(),
                        NrOfDoctoralCandidates = c.String(),
                    })
                .PrimaryKey(t => t.BenchmarkID);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Order = c.Int(nullable: false),
                        ParentCategoryID = c.Int(),
                        SurveyID = c.Int(),
                    })
                .PrimaryKey(t => t.CategoryID)
                .ForeignKey("dbo.Categories", t => t.ParentCategoryID)
                .ForeignKey("dbo.Surveys", t => t.SurveyID)
                .Index(t => t.ParentCategoryID)
                .Index(t => t.SurveyID);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionID = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        GoalsText = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        ExtraInfo1 = c.String(),
                        ExtraInfo2 = c.String(),
                        ExtraInfo3 = c.String(),
                        ExtraInfo4 = c.String(),
                        ExtraInfo5 = c.String(),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CategoryID = c.Int(nullable: false),
                        SurveyID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .ForeignKey("dbo.Surveys", t => t.SurveyID, cascadeDelete: true)
                .Index(t => t.CategoryID)
                .Index(t => t.SurveyID);
            
            CreateTable(
                "dbo.UserAnswers",
                c => new
                    {
                        UserAnswerID = c.Int(nullable: false, identity: true),
                        Answer = c.String(),
                        Goal = c.String(),
                        User = c.String(),
                        ExtraAnswer1 = c.String(),
                        ExtraAnswer2 = c.String(),
                        ExtraAnswer3 = c.String(),
                        ExtraAnswer4 = c.String(),
                        ExtraAnswer5 = c.String(),
                        QuestionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserAnswerID)
                .ForeignKey("dbo.Questions", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.Surveys",
                c => new
                    {
                        SurveyID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SurveyID);
            
            CreateTable(
                "dbo.Evaluations",
                c => new
                    {
                        EvaluationID = c.Int(nullable: false, identity: true),
                        User = c.String(),
                        InstituteName = c.String(),
                        FormatPresentationRating = c.Int(nullable: false),
                        ResultPresentationRating = c.Int(nullable: false),
                        UsefullnessRating = c.Int(nullable: false),
                        WouldRecommendToOthers = c.String(),
                        ImprovementSuggestions = c.String(),
                        EvaluationComments = c.String(),
                    })
                .PrimaryKey(t => t.EvaluationID);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        DisplayName = c.String(),
                        IsSubAccount = c.Boolean(),
                        ParentUserName = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "SurveyID", "dbo.Surveys");
            DropForeignKey("dbo.Categories", "SurveyID", "dbo.Surveys");
            DropForeignKey("dbo.Questions", "CategoryID", "dbo.Categories");
            DropForeignKey("dbo.UserAnswers", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.Categories", "ParentCategoryID", "dbo.Categories");
            DropIndex("dbo.UserAnswers", new[] { "QuestionID" });
            DropIndex("dbo.Questions", new[] { "SurveyID" });
            DropIndex("dbo.Questions", new[] { "CategoryID" });
            DropIndex("dbo.Categories", new[] { "SurveyID" });
            DropIndex("dbo.Categories", new[] { "ParentCategoryID" });
            DropTable("dbo.UserProfile");
            DropTable("dbo.Evaluations");
            DropTable("dbo.Surveys");
            DropTable("dbo.UserAnswers");
            DropTable("dbo.Questions");
            DropTable("dbo.Categories");
            DropTable("dbo.Benchmarks");
        }
    }
}

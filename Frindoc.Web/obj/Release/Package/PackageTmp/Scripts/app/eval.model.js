(function (ko, datacontext) {
    datacontext.userAnswer = userAnswer;
    datacontext.survey = survey;
    datacontext.question = question;
    datacontext.category = category;
    datacontext.benchmark = benchmark;
    datacontext.userProfile = userProfile;
    datacontext.evaluation = evaluation;

    function category(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.categoryId = data.CategoryID;
        self.title = ko.observable(data.Title);
        self.parentCategoryId = data.ParentCategoryID;
        self.categories = ko.observableArray(importCategories(data.ChildCategories));
        self.sortedCategories = ko.computed(function () {
            return self.categories().sort(function (left, right) {
                // sorting by order
                return left.order() == right.order() ? 0 : (left.order() < right.order() ? -1 : 1);
            });
        });

        self.questions = ko.observableArray(importQuestions(data.Questions));
        self.surveyId = data.SurveyID;
        self.weight = ko.observable(data.Weight || 1);
        self.order = ko.observable(data.Order);

        self.isActive = ko.observable(false);

        self.newQuestionText = ko.observable('');
        self.newCategoryText = ko.observable('');

        // Non-persisted properties
        self.errorMessage = ko.observable();

        saveChanges = function () {
            return datacontext.updateCategory(self);
        };

        // Auto-save when these properties change
        self.title.subscribe(saveChanges);
        self.weight.subscribe(saveChanges);
        self.order.subscribe(saveChanges);

        self.toJson = function () { return ko.toJSON(self) };

        self.deleteQuestion = function () {
            if (confirm('Are you sure?')) {
                var question = this;
                return datacontext.deleteQuestion(question)
                     .done(function () {
                         self.questions.remove(question);
                         window.evalApp.viewModel.onDataChanged();
                     });
            }
        };

        self.deleteCategory = function () {
            if (confirm('Are you sure?')) {
                var category = this;
                return datacontext.deleteCategory(category)
                     .done(function () {
                         self.categories.remove(category);
                         window.evalApp.viewModel.onDataChanged();
                     });
            }
        };

        self.moveUp = function (parent) {
            self.changeOrder(self, parent, 'up');
        }
        self.moveDown = function (parent) {
            self.changeOrder(self, parent, 'down');
        }
        self.isMoveUpAvailable = function (parent) { // is there an element higher up in the chain?
            var higher = ko.utils.arrayFirst(parent.categories(), function (i) { return i.order() < self.order() && i.categoryId != self.categoryId; });
            return higher != null;
        }
        self.isMoveDownAvailable = function (parent) { // is there an element below this one in the chain?
            var lower = ko.utils.arrayFirst(parent.categories(), function (i) { return i.order() > self.order() && i.categoryId != self.categoryId; });
            return lower != null;
        }
        self.changeOrder = function (favorite, parent, direction) {
            // create array of sortedCategories to calculate new order with
            var sortArr = [];
            var index = -1;
            for (var cat in parent.sortedCategories()) {
                sortArr.push(parent.sortedCategories()[cat].categoryId);
                if (favorite.categoryId == parent.sortedCategories()[cat].categoryId) { index = sortArr.length - 1; }
            }
            // remove the favorite from the index
            sortArr.splice(index, 1);
            // re-add the favorite item up or down depending on direction
            var newIndex = direction == 'up' ? index - 1 : index + 1;
            sortArr.splice(newIndex, 0, favorite.categoryId);

            // for each category, set the order to the (index+1) of the item in the array
            for (var catI in parent.sortedCategories()) {
                var cat = parent.sortedCategories()[catI];
                var newOrder = sortArr.indexOf(cat.categoryId) + 1;
                cat.order(newOrder);
            }
        }
    };

    function benchmark(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.benchmarkId = data.BenchmarkID;
        self.choice = ko.observable(data.Choice);
        self.clarification = ko.validatedObservable(data.Clarification);
        self.alternative = ko.validatedObservable(data.Alternative);
        self.user = data.User;
        self.responsiblePersonName = ko.validatedObservable(data.ResponsiblePersonName);
        self.instituteName = ko.validatedObservable(data.InstituteName);
        self.country = ko.validatedObservable(data.Country);
        self.nrOfDoctoralCandidates = ko.observable(data.NrOfDoctoralCandidates);
        self.nrOfUndergraduates = ko.observable(data.NrOfUndergraduates);

        // validation rules
        self.responsiblePersonName.extend({ required: true });
        self.instituteName.extend({ required: true });
        self.country.extend({ required: true });
        self.alternative.extend({
            required: {
                onlyIf: function () {
                    return self.choice() == "0";
                }
            }
        });
        self.clarification.extend({
            required: {
                onlyIf: function () {
                    return self.choice() == "1" || self.choice() == "3";
                }
            }
        });

        self.isValid = ko.computed(function () {
            return self.responsiblePersonName.isValid() &&
                   self.instituteName.isValid() &&
                   self.country.isValid() &&
                   self.clarification.isValid() &&
                   self.alternative.isValid();
        });

        self.reevaluateValidationRules = function () {
            self.responsiblePersonName.valueHasMutated();
            self.instituteName.valueHasMutated();
            self.country.valueHasMutated();
            self.clarification.valueHasMutated();
            self.alternative.valueHasMutated();
        }

        self.instituteName.subscribe(function () {
            // change page title with institution name
            setPageTitle(self.instituteName());
        });

        self.continueClick = function () {
            setTimeout(function () {
                self.reevaluateValidationRules();
                if (self.isValid()) //if valid, continue to the next tab
                    switchPanel(3, '#panelAnswerSurvey');
            }, 500);
        }

        // Non-persisted properties
        self.errorMessage = ko.observable();

        self.saveChanges = function () {
            // Only real accounts can update the benchmarks
            if(!window.evalApp.viewModel.isSubAccount()){
                if (typeof (self.benchmarkId) === 'undefined')
                    return datacontext.saveNewBenchmark(self);
                else
                    return datacontext.updateBenchmark(self);
            }
        };

        //when loaded: do this:
        setPageTitle(self.instituteName());

        // Auto-save when these properties change
        self.choice.subscribe(self.saveChanges);
        self.clarification.subscribe(self.saveChanges);
        self.alternative.subscribe(self.saveChanges);
        self.responsiblePersonName.subscribe(self.saveChanges);
        self.instituteName.subscribe(self.saveChanges);
        self.country.subscribe(self.saveChanges);
        self.nrOfDoctoralCandidates.subscribe(self.saveChanges);
        self.nrOfUndergraduates.subscribe(self.saveChanges);

        self.toJson = function () { return ko.toJSON(self) };
    };

    function userAnswer(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.userAnswerId = data.UserAnswerID;
        self.questionId = data.QuestionID;
        self.isRequired = ko.observable(data.IsRequired);
        self.user = data.User;
        self.answer = ko.observable(data.Answer);
        self.goal = ko.observable(data.Goal);
        self.extraanswer1 = ko.observable(data.ExtraAnswer1);
        self.extraanswer2 = ko.observable(data.ExtraAnswer2);
        self.extraanswer3 = ko.observable(data.ExtraAnswer3);
        self.extraanswer4 = ko.observable(data.ExtraAnswer4);
        self.extraanswer5 = ko.observable(data.ExtraAnswer5);

        // Non-persisted properties
        self.errorMessage = ko.observable();

        saveChanges = function () {
            window.evalApp.viewModel.onDataChanged();
            return datacontext.updateUserAnswer(self);
        };

        // Auto-save when these properties change
        //self.answer.subscribe(saveChanges);
        //self.goal.subscribe(saveChanges);
        //self.extraanswer1.subscribe(saveChanges);
        //self.extraanswer2.subscribe(saveChanges);
        //self.extraanswer3.subscribe(saveChanges);
        //self.extraanswer4.subscribe(saveChanges);
        //self.extraanswer5.subscribe(saveChanges);

        self.toJson = function () { return ko.toJSON(self) };
    };

    function question(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.error = ko.observable('');
        self.questionId = data.QuestionID || 0;
        self.categoryId = data.CategoryID || 0;
        self.surveyId = data.SurveyID;
        self.text = ko.observable(data.Text || '');
        self.goalstext = ko.observable(data.GoalsText || '');
        self.type = ko.observable(data.Type || 'Numeric');
        self.weight = ko.observable(data.Weight || 1);
        self.extrainfo1 = ko.observable(data.ExtraInfo1);
        self.extrainfo2 = ko.observable(data.ExtraInfo2);
        self.extrainfo3 = ko.observable(data.ExtraInfo3);
        self.extrainfo4 = ko.observable(data.ExtraInfo4);
        self.extrainfo5 = ko.observable(data.ExtraInfo5);
        self.extraanswer1 = ko.observable();
        self.extraanswer2 = ko.observable();
        self.extraanswer3 = ko.observable();
        self.extraanswer4 = ko.observable();
        self.extraanswer5 = ko.observable();

        getUserAnswerForQuestion = function () {
            // Find the answer text that was given
            var userAnswer = ko.utils.arrayFirst(window.evalApp.viewModel.userAnswers(), function (s) { return s.questionId == self.questionId; });
            if (userAnswer) {
                return userAnswer;
            }
        }
        if (getUserAnswerForQuestion()) {
            self.extraanswer1(getUserAnswerForQuestion().extraanswer1());
            self.extraanswer2(getUserAnswerForQuestion().extraanswer2());
            self.extraanswer3(getUserAnswerForQuestion().extraanswer3());
            self.extraanswer4(getUserAnswerForQuestion().extraanswer4());
            self.extraanswer5(getUserAnswerForQuestion().extraanswer5());
        }

        getAnswerForQuestion = function () {
            // Find the answer text that was given
            var answer = '0';
            var userAnswer = ko.utils.arrayFirst(window.evalApp.viewModel.userAnswers(), function (s) { return s.questionId == self.questionId; });
            if (userAnswer) {
                answer = userAnswer.answer();
            }
            return parseInt(answer, 10);
        }

        getGoalForQuestion = function () {
            // Find the answer text that was given
            var goal = '0';
            var userAnswer = ko.utils.arrayFirst(window.evalApp.viewModel.userAnswers(), function (s) { return s.questionId == self.questionId; });
            if (userAnswer) {
                goal = userAnswer.goal();
            }
            return parseInt(goal, 0);
        }

        self.answer = ko.validatedObservable(getAnswerForQuestion());
        self.goal = ko.validatedObservable(getGoalForQuestion());

        // Non-persisted properties
        self.errorMessage = ko.observable();

        self.saveChanges = function () {
            return datacontext.updateQuestion(self)
                        .then(addSucceeded)
                        .fail(addFailed);

            function addSucceeded() {
                window.evalApp.viewModel.onDataChanged();
            }
            function addFailed() {
                self.error("Save of question failed");
            }
        };

        self.saveAnswer = function () {
            // validate answer & goal
            if ((typeof self.answer.isValid !== 'undefined' && self.answer.isValid()) &&
                (typeof self.goal.isValid !== 'undefined' && self.goal.isValid())) {
                // Create/Update the answer when filled in
                var userAnswer = ko.utils.arrayFirst(window.evalApp.viewModel.userAnswers(), function (s) { return s.questionId == self.questionId; });
                if (userAnswer) { //Update
                    userAnswer.answer(self.answer());
                    userAnswer.goal(self.goal());
                    userAnswer.extraanswer1(self.extraanswer1());
                    userAnswer.extraanswer2(self.extraanswer2());
                    userAnswer.extraanswer3(self.extraanswer3());
                    userAnswer.extraanswer4(self.extraanswer4());
                    userAnswer.extraanswer5(self.extraanswer5());
                    datacontext.updateUserAnswer(userAnswer);

                    window.evalApp.viewModel.onDataChanged(); //refresh grid
                } else if (self.questionId > 0) {
                    userAnswer = datacontext.createUserAnswer({ QuestionID: self.questionId, SurveyID: self.surveyId, Answer: self.answer(), Goal: self.goal() });
                    datacontext.saveNewUserAnswer(userAnswer);
                }
            }
        };

        self.setValidationRules = function () {
            if (self.type() == 'Numeric') {
                self.answer.extend({ min: 0, max: 5, number: true });
                self.goal.extend({ min: 0, max: 5, number: true });
            } else {
                self.answer.extend({ validatable: false });
                self.goal.extend({ validatable: false });
            }
        };

        // Auto-save when these properties change
        self.text.subscribe(self.saveChanges);
        self.type.subscribe(self.saveChanges);
        self.extrainfo1.subscribe(self.saveChanges);
        self.extrainfo2.subscribe(self.saveChanges);
        self.extrainfo3.subscribe(self.saveChanges);


        // Set validation rules based on the type
        self.type.subscribe(self.setValidationRules);

        // Create/Update the answer when filled in
        self.answer.subscribe(self.saveAnswer);
        self.goal.subscribe(self.saveAnswer);
        self.extraanswer1.subscribe(self.saveAnswer);
        self.extraanswer2.subscribe(self.saveAnswer);
        self.extraanswer3.subscribe(self.saveAnswer);
        self.extraanswer4.subscribe(self.saveAnswer);
        self.extraanswer5.subscribe(self.saveAnswer);

        self.toJson = function () { return ko.toJSON(self) };

        //initialy set validationRules
        self.setValidationRules();
    };

    function survey(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.surveyId = data.SurveyID;
        self.title = ko.observable(data.Title || "My Survey");
        self.categories = ko.observableArray(importCategories(data.Categories));
        self.sortedCategories = ko.computed(function () {
            return self.categories().sort(function (left, right) {
                return left.order() == right.order() ? 0 : (left.order() < right.order() ? -1 : 1);
            });
        });
        self.errorMessage = ko.observable();
        self.isActiveForEditing = ko.observable(false);
        self.isActiveForAnswering = ko.observable(false);
        self.toJson = function () { return ko.toJSON(self) };

        self.newCategoryText = ko.observable('');

        self.openSurveyForEditing = function () {
            self.isActiveForEditing(true);
            displayPanel("#panelManageSurvey");
        }

        self.openSurveyForAnswering = function () {
            self.isActiveForAnswering(true);
            displayPanel("#panelChooseBenchmark");
        }

        self.deleteCategory = function () {
            if (confirm('Are you sure?')) {
                var category = this;
                return datacontext.deleteCategory(category)
                     .done(function () {
                         self.categories.remove(category);
                         window.evalApp.viewModel.onDataChanged();
                     });
            }
        };

        //self.openSurveyForAnswering();
    };

    function evaluation(data) {
        var self = this;
        data = data || {};

        // Persisted properties
        self.instituteName = ko.observable(data.InstituteName);
        self.formatPresentationRating = ko.observable(data.FormatPresentationRating);
        self.resultPresentationRating = ko.observable(data.ResultPresentationRating);
        self.usefullnessRating = ko.observable(data.UsefullnessRating)
        self.wouldRecommendToOthers = ko.observable(data.WouldRecommendToOthers)
        self.improvementSuggestions = ko.observable(data.ImprovementSuggestions)
        self.evaluationComments = ko.observable(data.EvaluationComments)
        self.evaluationId = data.EvaluationID;

        self.errorMessage = ko.observable();

        saveChanges = function () {
            // Only real accounts can update the benchmarks
            if (!window.evalApp.viewModel.isSubAccount()) {
                if (typeof (self.evaluationId) === 'undefined')
                    return datacontext.saveNewEvaluation(self);
                else
                    return datacontext.updateEvaluation(self);
            }
        };

        // Auto-save when these properties change
        self.instituteName.subscribe(saveChanges);
        self.formatPresentationRating.subscribe(saveChanges);
        self.resultPresentationRating.subscribe(saveChanges);
        self.usefullnessRating.subscribe(saveChanges);
        self.wouldRecommendToOthers.subscribe(saveChanges);
        self.improvementSuggestions.subscribe(saveChanges);
        self.evaluationComments.subscribe(saveChanges);

        self.toJson = function () { return ko.toJSON(self) };
    }

    function userProfile(data) {
        var self = this;
        data = data || {};

        self.emailaddress = ko.observable(data.UserName); // email
        self.username = ko.observable(data.DisplayName); // username
        self.isSubAccount = ko.observable(data.IsSubAccount);
        self.errorMessage = ko.observable('');

        self.toJson = function () { return ko.toJSON(self) };
    };

    // convert raw category data objects into array of Categories
    function importCategories(data) {
        /// <returns value="[new category()]"></returns>
        return $.map(data || [],
                function (categoryData) {
                    return datacontext.createCategory(categoryData);
                });
    }

    // convert raw question data objects into array of Questions
    function importQuestions(data) {
        /// <returns value="[new question()]"></returns>
        return $.map(data || [],
                function (questionData) {
                    return datacontext.createQuestion(questionData);
                });
    }

    // convert raw userAnswer data objects into array of UserAnswers
    function importAnswers(data) {
        /// <returns value="[new userAnswer()]"></returns>
        return $.map(data || [],
                function (answerData) {
                    return datacontext.createUserAnswer(answerData);
                });
    }

    category.prototype.addQuestion = function () {
        var self = this;
        if (self.newQuestionText()) { // need a text to save
            var question = datacontext.createQuestion(
                {
                    Text: self.newQuestionText(),
                    Type: '',
                    SurveyID: window.evalApp.viewModel.activeEditingSurvey().surveyId,
                    CategoryID: self.categoryId
                });
            datacontext.saveNewQuestion(question);
            self.questions.push(question);
            self.newQuestionText("");

            window.evalApp.viewModel.onDataChanged();
        }
    };

    category.prototype.addCategory = function () {
        var self = this;
        if (self.newCategoryText()) { // need a text to save
            var category = datacontext.createCategory(
                {
                    Title: self.newCategoryText(),
                    //SurveyID: window.evalApp.viewModel.activeEditingSurvey().surveyId,
                    ParentCategoryID: self.categoryId
                });
            datacontext.saveNewCategory(category);
            self.categories.push(category);
            self.newCategoryText("");

            window.evalApp.viewModel.onDataChanged();
        }
    };

    survey.prototype.addCategory = function () {
        var self = this;
        if (self.newCategoryText()) { // need a text to save
            var category = datacontext.createCategory(
                {
                    Title: self.newCategoryText(),
                    SurveyID: self.surveyId
                });
            datacontext.saveNewCategory(category);
            self.categories.push(category);
            self.newCategoryText("");

            window.evalApp.viewModel.onDataChanged();
        }
    };

})(ko, evalApp.datacontext);
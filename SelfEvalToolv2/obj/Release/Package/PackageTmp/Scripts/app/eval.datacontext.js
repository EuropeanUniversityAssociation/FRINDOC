window.evalApp = window.evalApp || {};

window.evalApp.datacontext = (function () {

    var datacontext = {
        getSurveys: getSurveys,
        getQuestions: getQuestions,
        getUserAnswers: getUserAnswers,
        getCategories: getCategories,
        getBenchmark: getBenchmark,
        getSubAccounts: getSubAccounts,
        getEvaluation: getEvaluation,
        createQuestion: createQuestion,
        createUserAnswer: createUserAnswer,
        createSurvey: createSurvey,
        createCategory: createCategory,
        createBenchmark: createBenchmark,
        createEvaluation: createEvaluation,
        saveNewUserAnswer: saveNewUserAnswer,
        saveNewQuestion: saveNewQuestion,
        saveNewCategory: saveNewCategory,
        saveNewBenchmark: saveNewBenchmark,
        saveNewSubAccount: saveNewSubAccount,
        saveNewEvaluation: saveNewEvaluation,
        updateUserAnswer: updateUserAnswer,
        updateQuestion: updateQuestion,
        updateCategory: updateCategory,
        updateBenchmark: updateBenchmark,
        updateEvaluation: updateEvaluation,
        deleteUserAnswer: deleteUserAnswer,
        deleteQuestion: deleteQuestion,
        deleteCategory: deleteCategory,
        deleteBenchmark: deleteBenchmark,
        deleteSubAccount: deleteSubAccount
    };

    return datacontext;

    function getSurveys(surveys, errorObservable, onDone) {
        return ajaxRequest("get", surveyUrl())
          .done(getSucceeded)
          .fail(getFailed);

        function getSucceeded(serverData) {
            var mappedSurveys = $.map(serverData, function (val, i) { return new createSurvey(val); });
            surveys(mappedSurveys);
            if (onDone) onDone();
        }

        function getFailed() {
            errorObservable("Error retrieving surveys.");
        }
    }
    function createUserAnswer(serverData) {
        return new datacontext.userAnswer(serverData); // todoItem is injected by todo.model.js
    }
    function createSurvey(serverData) {
        return new datacontext.survey(serverData); // survey is injected by todo.model.js
    }
    function createQuestion(serverData) {
        return new datacontext.question(serverData); // question is injected by todo.model.js
    }
    function createCategory(serverData) {
        return new datacontext.category(serverData); // category is injected by todo.model.js
    }
    function createBenchmark(serverData) {
        return new datacontext.benchmark(serverData); // benchmark is injected by todo.model.js
    }
    function createEvaluation(serverData) {
        return new datacontext.evaluation(serverData); // evaluation is injected by todo.model.js
    }
    function createUserProfile(serverData) {
        return new datacontext.userProfile(serverData); // survey is injected by todo.model.js
    }
    function getQuestions(questionObservable, errorObservable, onDone) {
        return ajaxRequest("get", questionUrl())
         .done(getSucceeded)
         .fail(getFailed);

        function getSucceeded(serverData) {
            var mappedQuestions = $.map(serverData, function (val, i) { return new createQuestion(val); });
            questionObservable(mappedQuestions);
            if (onDone) onDone();
        }

        function getFailed() {
            errorObservable("Error retrieving questions.");
        }
    }
    function getCategories(categoryObservable, errorObservable, onDone) {
        return ajaxRequest("get", categoryUrl())
         .done(getSucceeded)
         .fail(getFailed);

        function getSucceeded(serverData) {
            var mappedCategories = $.map(serverData, function (val, i) { return new createCategory(val); });
            categoryObservable(mappedCategories);
            if (onDone) onDone();
        }

        function getFailed() {
            errorObservable("Error retrieving categories.");
        }
    }
    function getUserAnswers(userAnswerObservable, errorObservable, onDone) {
        return ajaxRequest("get", userAnswerUrl())
         .done(getSucceeded)
         .fail(getFailed);

        function getSucceeded(serverData) {
            var mappedUserAnswers = $.map(serverData, function (val, i) { return new createUserAnswer(val) });
            userAnswerObservable(mappedUserAnswers);
            if(onDone) onDone();
        }

        function getFailed(ex) {
            errorObservable("Error retrieving your answers." + ex.responseText);
        }
    }
    function getBenchmark(benchmarkObservable, errorObservable, onDone) {
        return ajaxRequest("get", benchmarkUrl())
         .done(getSucceeded)
         .fail(getFailed);

        function getSucceeded(serverData) {
            benchmarkObservable(new createBenchmark(serverData));
            if (onDone) onDone();
        }

        function getFailed(ex) {
            errorObservable("Error retrieving your benchmark data." + ex.responseText);
        }
    }
    function getEvaluation(evalautionObservable, errorObservable, onDone) {
        return ajaxRequest("get", evaluationUrl())
         .done(getSucceeded)
         .fail(getFailed);

        function getSucceeded(serverData) {
            evalautionObservable(new createEvaluation(serverData));
            if (onDone) onDone();
        }

        function getFailed(ex) {
            errorObservable("Error retrieving your evaluation data." + ex.responseText);
        }
    }
    function getSubAccounts(userprofiles, errors, onDone) {
        return ajaxRequest("get", userProfilesUrl())
          .done(getSucceeded)
          .fail(getFailed);

        function getSucceeded(serverData) {
            var mappedUserProfiles = $.map(serverData, function (val, i) { return new createUserProfile(val); });
            userprofiles(mappedUserProfiles);
            if (onDone) onDone();
        }

        function getFailed() {
            errorObservable("Error retrieving userprofiles.");
        }
    }
    function saveNewUserAnswer(userAnswer) {
        clearErrorMessage(userAnswer);
        return ajaxRequest("post", userAnswerUrl(), userAnswer)
            .done(function (result) {
                userAnswer.userAnswerId = result.UserAnswerID;
                userAnswer.user = result.User;
                window.evalApp.viewModel.userAnswers().push(userAnswer);
            })
            .fail(function (ex) {
                userAnswer.errorMessage("Problem saving your answer." + ex.responseText);
            });
    }
    function saveNewQuestion(question) {
        clearErrorMessage(question);
        return ajaxRequest("post", questionUrl(), question)
            .done(function (result) {
                question.questionId = result.QuestionID;
            })
            .fail(function (ex) {
                question.errorMessage("Error adding a new question." + ex.responseText);
            });
    }
    function saveNewCategory(category) {
        clearErrorMessage(category);
        return ajaxRequest("post", categoryUrl(), category)
            .done(function (result) {
                category.categoryId = result.CategoryID;
                category.order(result.Order);
            })
            .fail(function (ex) {
                category.errorMessage("Error adding a new category." + ex.responseText);
            });
    }
    function saveNewBenchmark(benchmark) {
        clearErrorMessage(benchmark);
        return ajaxRequest("post", benchmarkUrl(), benchmark)
            .done(function (result) {
                benchmark.benchmarkId = result.BenchmarkID;
            })
            .fail(function (ex) {
                benchmark.errorMessage("Error adding a new benchmark." + ex.responseText);
            });
    }
    function saveNewSubAccount(userProfile, onDone, onFailed) {
        clearErrorMessage(userProfile);
        return ajaxRequest("post", registerUserProfileUrl(), userProfile)
            .done(function (result) {
                if (onDone) onDone();
            })
            .fail(function (ex) {
                var properError = '';
                try {
                    properError = ((JSON.parse(ex.responseText)).ModelState.email[0]);
                    if (typeof (properError) !== 'undefined' && properError.length > 0)
                        userProfile.errorMessage(properError); if (onFailed) onFailed(); return;
                } catch (e) { }
                try {
                    properError = ((JSON.parse(ex.responseText)).ModelState['email'][0]);
                    if (typeof (properError) !== 'undefined' && properError.length > 0)
                        userProfile.errorMessage(properError); if (onFailed) onFailed(); return;
                } catch (e){ }
                try {
                    properError = ((JSON.parse(ex.responseText)).ModelState['model.Username']);
                    if (typeof (properError) !== 'undefined' && properError.length > 0)
                        userProfile.errorMessage(properError); if (onFailed) onFailed(); return;
                } catch (e) { }
                try {
                    properError = ((JSON.parse(ex.responseText)).ModelState['model.EmailAddress'][0]);
                    if (typeof (properError) !== 'undefined' && properError.length > 0)
                        userProfile.errorMessage(properError); if (onFailed) onFailed(); return;
                } catch (e) { }

                if (onFailed) onFailed();
            });
    }
    function saveNewEvaluation(evaluation) {
        clearErrorMessage(evaluation);
        return ajaxRequest("post", evaluationUrl(), evaluation)
            .done(function (result) {
                evaluation.evaluationId = result.EvaluationID;
            })
            .fail(function (ex) {
                evaluation.errorMessage("Error adding a new evaluation." + ex.responseText);
            });
    }
    function updateUserAnswer(userAnswer) {
        clearErrorMessage(userAnswer);
        return ajaxRequest("put", userAnswerUrl(userAnswer.userAnswerId), userAnswer, "text")
            .fail(function (ex) {
                userAnswer.errorMessage("Error saving your answer." + ex.responseText);
            });
    }
    function updateQuestion(question) {
        clearErrorMessage(question);
        return ajaxRequest("put", questionUrl(question.questionId), question, "text")
            .fail(function (ex) {
                question.errorMessage("Error saving question:" + ex.responseText);
            });
    }
    function updateCategory(category) {
        clearErrorMessage(category);
        return ajaxRequest("put", categoryUrl(category.categoryId), category, "text")
            .fail(function (ex) {
                category.errorMessage("Error saving category:" + ex.responseText);
            });
    }
    function updateBenchmark(benchmark) {
        clearErrorMessage(benchmark);
        return ajaxRequest("put", benchmarkUrl(benchmark.benchmarkId), benchmark, "text")
            .fail(function (ex) {
                benchmark.errorMessage("Error saving benchmark:" + ex.responseText);
            });
    }
    function updateEvaluation(evaluation) {
        clearErrorMessage(evaluation);
        return ajaxRequest("put", evaluationUrl(evaluation.evaluationId), evaluation, "text")
            .fail(function (ex) {
                evaluation.errorMessage("Error saving evaluation:" + ex.responseText);
            });
    }
    function deleteUserAnswer(userAnswer) {
        return ajaxRequest("delete", userAnswerUrl(userAnswer.userAnswerId))
            .fail(function () {
                userAnswer.errorMessage("Error removing your answer.");
            });
    }
    function deleteQuestion(question) {
        return ajaxRequest("delete", questionUrl(question.questionId))
            .fail(function (ex) {
                question.errorMessage("Error removing question." + ex.responseText);
            });
    }
    function deleteCategory(category) {
        return ajaxRequest("delete", categoryUrl(category.categoryId))
            .fail(function (ex) {
                category.errorMessage("Error removing category." + ex.responseText);
            });
    }
    function deleteBenchmark(benchmark) {
        return ajaxRequest("delete", benchmarkUrl(benchmark.benchmarkId))
            .fail(function (ex) {
                benchmark.errorMessage("Error removing benchmark." + ex.responseText);
            });
    }
    function deleteSubAccount(userProfile, onDone, onFailed) {
        clearErrorMessage(userProfile);
        return ajaxRequest("delete", deleteUserProfileUrl(), userProfile)
            .done(function (result) {
                if (onDone) onDone();
            })
            .fail(function (ex) {
                if (onFailed) onFailed();
            });
    }

    // Private
    function clearErrorMessage(entity) { entity.errorMessage(null); }
    function ajaxRequest(type, url, data, dataType) { // Ajax helper
        var options = {
            dataType: dataType || "json",
            contentType: "application/json; charset=utf-8",
            cache: false,
            type: type,
            data: data ? data.toJson() : null,
            error: function (xhr, ajaxOptions, thrownError) {
                if (data.errorMessage)
                    data.errorMessage('Sorry the server is currently not available.\r\n' + thrownError);
                //else if (console && console.log)
                //    console.log(thrownError);
            }
        };
        var antiForgeryToken = $("#antiForgeryToken").val();
        if (antiForgeryToken) {
            options.headers = {
                'RequestVerificationToken': antiForgeryToken
            }
        }
        return $.ajax(url, options);
    }
    // routes
    function userAnswerUrl(id) { return "/api/useranswer/" + (id || ""); }
    function questionUrl(id) { return "/api/question/" + (id || ""); }
    function surveyUrl(id) { return "/api/survey/" + (id || ""); }
    function categoryUrl(id) { return "/api/category/" + (id || ""); }
    function benchmarkUrl(id) { return "/api/benchmark/" + (id || ""); }
    function evaluationUrl(id) { return "/api/evaluation/" + (id || ""); }
    function userProfilesUrl() { return "/api/userprofile/GetSubUserProfiles"; }
    function registerUserProfileUrl() { return "/api/userprofile/CreateSubAccount"; }
    function deleteUserProfileUrl() { return "/api/userprofile/DeleteSubAccount"; }
})();

window.evalApp.viewModel = (function (ko, datacontext) {
    var self = this;
    self.surveys = ko.observableArray();
    self.userAnswers = ko.observableArray();
    self.benchmark = ko.observable();
    self.evaluation = ko.observable();
    self.isSubAccount = ko.observable();
    self.subAccounts = ko.observableArray();
    self.errors = ko.observableArray();
    self.activeCategory = ko.observable();
    self.activeParentCategory = ko.observable();
    self.categoryNavigateDirection = ko.observable('left');
    self.previousCategory = ko.observable();
    self.nextCategory = ko.observable();
    self.activeEditingSurvey = ko.dependentObservable(function () {
        if(self.surveys().length > 0) {
            return ko.utils.arrayFirst(self.surveys(), function (s) { return s.isActiveForEditing; }) || self.surveys()[0];
        } else {
            return datacontext.createSurvey();
        }
    }, self);
    self.activeAnsweringSurvey = ko.dependentObservable(function () {
        if (self.surveys().length > 0) {
            return ko.utils.arrayFirst(self.surveys(), function (s) { return s.isActiveForAnswering; }) || self.surveys()[0];
        } else {
            return datacontext.createSurvey();
        }
    }, self);
    self.newSubAccount = ko.observable(new datacontext.userProfile());
    self.isCreatingSubAccount = ko.observable(false);

    self.availableQuestionTypes = ko.observableArray(['Numeric', 'Text', 'Yes/No'])
    
    self.navigateCategory = function (category) {
        self.setActiveCategory(category);
    }
    self.navigateParentCategory = function (category) {
        self.setActiveParentCategory(category);
    }

    self.startTool = function () {
        if (self.benchmark().isValid()) {
            switchPanel(3, '#panelAnswerSurvey');
        } else {
            switchPanel(2, '#panelChooseBenchmark');
        }
    }

    self.setPreviousCategoryActive = function () {
        var previous = self.findPreviousCategory(self.activeAnsweringSurvey(), self.activeCategory());
        if (previous.parentCategoryId == null)
            self.navigateParentCategory(previous);
        else
            self.navigateCategory(previous);
    }
    self.setNextCategoryActive = function () {
        var next = self.findNextCategory(self.activeAnsweringSurvey(), self.activeCategory());
        if(next.parentCategoryId == null)
            self.navigateParentCategory(next);
        else
            self.navigateCategory(next);
    }

    self.isPreviousCategoryAvailable = ko.computed(function () {
        if (self.activeAnsweringSurvey() && self.activeCategory()) {
            var previous = self.findPreviousCategory(self.activeAnsweringSurvey(), self.activeCategory());
            return typeof (previous) !== 'undefined' && previous != null;
        }
        return false;
    });

    self.isNextCategoryAvailable = ko.computed(function () {
        if (self.activeAnsweringSurvey() && self.activeCategory()) {
            var next = self.findNextCategory(self.activeAnsweringSurvey(), self.activeCategory());
            return (typeof (next) !== 'undefined' && next != null);
        }
        return false;
    });

    self.findNextCategory = function (parent, objToFind) {
        var toReturn = null;
        var takeTheNextOne = false;
        $(parent.categories()).each(function (i, n) {
            if (toReturn != null) return false;
            if (takeTheNextOne) { toReturn = n; return false; }
            if(n == objToFind) takeTheNextOne = true;
            $(n.categories()).each(function(i, n2){
                if (takeTheNextOne) { toReturn = n2; return false; }
                if(n2 == objToFind) takeTheNextOne = true;
            });
        });
        self.nextCategory(toReturn);
        return toReturn;
    }

    self.findPreviousCategory = function (parent, objToFind) {
        var takeTheNextOne = false;
        for (var i = parent.categories().length - 1; i >= 0; i--) {
            var obj = parent.categories()[i];
            if (takeTheNextOne) { self.previousCategory(obj); return obj; }
            if (obj == objToFind) takeTheNextOne = true;
            for (var j = obj.categories().length - 1; j >= 0; j--) {
                var sobj = obj.categories()[j];
                if (takeTheNextOne) { self.previousCategory(sobj); return sobj; }
                if (sobj == objToFind) takeTheNextOne = true;
            }
        }
    }

    self.setActiveCategory = function (category) {
        if (self.activeCategory() != category) {

            if (self.activeCategory()) {
                self.activeCategory().isActive(false);
                if (self.activeParentCategory().categoryId == self.activeCategory().parentCategoryId) {
                    if (self.activeCategory().order() >= category.order())
                        self.categoryNavigateDirection('left');
                    else
                        self.categoryNavigateDirection('right');
                }
            }
            self.activeCategory(category);
            category.isActive(true);
        }
    }

    self.setActiveParentCategory = function (category) {
        if (self.activeParentCategory() != category) {
            if (self.activeParentCategory()) {
                self.activeParentCategory().isActive(false);
                if (self.activeParentCategory().order() >= category.order())
                    self.categoryNavigateDirection('left');
                else
                    self.categoryNavigateDirection('right');
            }
            self.activeParentCategory(category);
            category.isActive(true);
            self.setActiveCategory(category.categories()[0]);
        }
    }

    self.validateNewSubAccount = function () {
        if (typeof(self.newSubAccount().username()) == 'undefined' || self.newSubAccount().username() == '') {
            self.newSubAccount().errorMessage('Please enter a Name.');
            return false;
        }
        if (typeof(self.newSubAccount().emailaddress()) == 'undefined' || self.newSubAccount().emailaddress() == '') {
            self.newSubAccount().errorMessage('Please enter a valid email address.');
            return false;
        }
        return true;
    }

    self.createSubAccount = function () {
        if (self.validateNewSubAccount()) {
            self.isCreatingSubAccount(true);
            datacontext.saveNewSubAccount(self.newSubAccount(), function () {
                self.isCreatingSubAccount(false);
                self.refreshSubAccounts();
                self.newSubAccount(new datacontext.userProfile());
            }, function () {
                self.isCreatingSubAccount(false);
            });
        }
    }

    self.deleteSubAccount = function (account) {
        if (confirm('Are you sure you want to delete this subaccount?')) {
            datacontext.deleteSubAccount(account, function () {
                self.refreshSubAccounts();
            }, function () {
                self.refreshSubAccounts();
            });
        }
    }

    self.refreshSubAccounts = function () {
        self.subAccounts([]); //clear all
        if (!self.isSubAccount()) {
            datacontext.getSubAccounts(self.subAccounts, errors, function () {
                // on done
            });
        }
    }

    // Fix PM 20140506 - Useranswers needs to be loaded before all else...
    datacontext.getUserAnswers(userAnswers, errors, function () {
        datacontext.getSurveys(surveys, errors, function () {
            datacontext.getBenchmark(benchmark, errors, function () {
                datacontext.getEvaluation(self.evaluation, errors, function () {
                    self.setActiveParentCategory(self.activeAnsweringSurvey().categories()[0]);
                    self.setActiveCategory(self.activeAnsweringSurvey().categories()[0].categories()[0]);
                    self.refreshSubAccounts();
                })
            });
        });
    });

    activeAnsweringSurvey.subscribe(function () {
        window.evalApp.highchart.onSurveyChanged(self.activeAnsweringSurvey());
    });

    self.onDataChanged = function () {
        if (window.evalApp.highchart) // update highcharts if available
            window.evalApp.highchart.onSurveyChanged(self.activeAnsweringSurvey());
    };

    self.benchmark.subscribe(function () {
        // hide the loader when this is initialized
        $('#loader').hide();
    });

    self.init = function () {
        self.isSubAccount($("#isSubAccount").val() == 'true');
    }

    self.init();

    // public accessors
    return {
        surveys: surveys,
        benchmark: benchmark,
        activeEditingSurvey: activeEditingSurvey,
        activeAnsweringSurvey: activeAnsweringSurvey,
        availableQuestionTypes: availableQuestionTypes,
        userAnswers: userAnswers,
        onDataChanged: onDataChanged,
        navigateCategory: navigateCategory,
        navigateParentCategory: navigateParentCategory,
        activeCategory: activeCategory,
        activeParentCategory: activeParentCategory,
        categoryNavigateDirection: categoryNavigateDirection,
        setPreviousCategoryActive: setPreviousCategoryActive,
        setNextCategoryActive: setNextCategoryActive,
        isPreviousCategoryAvailable: isPreviousCategoryAvailable,
        isNextCategoryAvailable: isNextCategoryAvailable,
        previousCategory: previousCategory,
        nextCategory: nextCategory,
        startTool: startTool,
        createSubAccount: createSubAccount,
        deleteSubAccount: deleteSubAccount,
        isSubAccount: isSubAccount
    };

})(ko, evalApp.datacontext);

ko.validation.init({
    parseInputAttributes: false,
});
ko.applyBindings(window.evalApp.viewModel);

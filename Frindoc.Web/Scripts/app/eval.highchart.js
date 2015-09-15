window.evalApp.highchart = new function () {
    var self = this;
    self.categories = [];
    self.series = [[],[]];

    self.onSurveyChanged = function (survey) {
        // recalculate data
        self.calculateChartData(survey);
        self.refreshChart();
    }
    self.calculateWeightedAnswer = function (category, isGoal) {
        //[sum(category_with_answers.answers) * category_with_answers.weight]
        var sum = 0;
        var weight = category.weight();
        $(category.questions()).each(function (questionIndex, question) {
            // Process only questions with numeric type and with userAnswer
            if (question.type() == 'Numeric') {
                if (isGoal)
                    sum += parseFloat(question.goal(), 10);
                else
                    sum += parseFloat(question.answer(), 10);
            }
        });
        return sum * weight;
    }
    self.calculateChartData = function (survey) {
        // 
        // { series = foreach:category_with_answers => [sum(category_with_answers.answers) * category_with_answers.weight] }
        // { categories = foreach:category_with_answers => [name: category_with_answers.title] }
        // 

        // reset series and categories
        self.series = [[], []];
        self.categories = [];
        self.calculateChartDataRecursive(survey);
    }

    self.calculateChartDataRecursive = function (parent) {
        $(parent.categories()).each(function (catIndex, cat) {
            if (typeof cat.questions() !== 'undefined' && cat.questions().length > 0) {
                if (self.categories.indexOf(cat.title()) == -1)
                    self.categories.push(cat.title());
                // answers
                self.series[0].push(self.calculateWeightedAnswer(cat, false));
                // goals
                self.series[1].push(self.calculateWeightedAnswer(cat, true));
            }
            if (typeof cat.categories() !== 'undefined' && cat.categories().length > 0)
                self.calculateChartDataRecursive(cat);
        });
    }

    self.refreshChart = function (chartCategories, chartSeries) {
        var chart = $('#highchart').highcharts();
        if (typeof (chart) !== 'undefined') {
            // Set categories
            chart.xAxis[0].setCategories(self.categories);
            
            // Set colors
            chart.series[0].color = '#0066FF';
            chart.series[1].color = '#FF0000';
            chart.legend.colorizeItem(chart.series[0], chart.series[0].visible);
            chart.legend.colorizeItem(chart.series[1], chart.series[1].visible);

            // Set series data
            chart.series[0].setData(self.series[0]);//status
            chart.series[1].setData(self.series[1]);//goal


            canvg(document.getElementById('canvas'), chart.getSVG())

            var canvas = document.getElementById("canvas");
            var img = canvas.toDataURL("image/png");

            if ($("canvas").length > 0) {
                $("#highcharts-png").attr('src', img);
            }
        }
    }
};

$(function () {
    $('#highchart').highcharts({

        chart: {
            polar: true,
            type: 'line'
        },

        title: {
            text: 'FRINDOC',
            x: -80
        },

        pane: {
            size: '80%'
        },

        xAxis: {
            categories: ["No categories loaded"],
            tickmarkPlacement: 'on',
            lineWidth: 0
        },

        yAxis: {
            gridLineInterpolation: 'polygon',
            lineWidth: 0,
            min: 0,
            max: 10
        },

        tooltip: {
            shared: true,
            pointFormat: '<span style="color:{series.color}">{series.name}: <b>{point.y:,.0f}</b><br/>'
        },

        legend: {
            align: 'right',
            verticalAlign: 'top',
            y: 70,
            layout: 'vertical',
            floating: true
        },

        series: [{
            name: 'Status',
            data: [0],

        }, {
            name: 'Goal',
            data: [1],
        }]
    });
});
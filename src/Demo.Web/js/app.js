String.prototype.replaceAll = function(str1, str2, ignore) 
{
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g,"\\$&"),(ignore?"gi":"g")),(typeof(str2)=="string")?str2.replace(/\$/g,"$$$$"):str2);
}

var app = angular.module("Zupla", ["ng"]);

app.controller("newPropertyCtrl", function ($scope, $http) {
    $scope.bedrooms = "2";
    $scope.bathrooms = "1";
    $scope.operationType = "Sale";
    $scope.propertyType = "Apartment";
    $scope.distanceToStation = 500;
    $scope.squareMts = "";
    $scope.age = 20;
    $scope.zoneReputation = "Good";
    $scope.hasParking = false;
    $scope.withFurniture = false;
    $scope.hasBackyard = false;
    $scope.price = "";

    $scope.$invalidPrice = false;
    $scope.$priceUnknown = false;
    $scope.$priceValidationMessage = "";

    $scope.$invalidSize = false;
    $scope.$unknownSize = false;
    $scope.$sizeValidationMessage = "";
    $scope.$logs = [];

    $scope.$watch("bedrooms", function () {
        delayExecution(validatePriceAndSize);
    });

    $scope.$watch("bathrooms", function () {
        delayExecution(validatePriceAndSize);
    });

    $scope.$watch("squareMts", function () {
        delayExecution(validatePriceAndSize);
    });

    $scope.$watch("operationType", function () {
        delayExecution(validatePrice);
    });

    $scope.$watch("propertyType", function () {
        delayExecution(validatePriceAndSize);
    });

    $scope.$watch("distanceToStation", function () {
        delayExecution(validatePrice);
    });

    $scope.$watch("age", function () {
        delayExecution(validatePrice);
    });

    $scope.$watch("zoneReputation", function () {
        delayExecution(validatePrice);
    });

    $scope.$watch("hasParking", function () {
        delayExecution(validatePrice);
    });

    $scope.$watch("price", function () {
        delayExecution(validatePrice);
    });

    function validatePriceAndSize() {
        validatePrice();
        validateSize();
    }

    function validatePrice() {
        $scope.$invalidPrice = false;
        $scope.$priceUnknown = false;
        $scope.$priceValidationMessage = "";

        if (!$scope.price) return;

        var param = $.param(getParams());
        var url = "api/property/price/predict?" + param;
        $http.get(url).then(function (response) {
            processPricePrediction(response.data);
            $scope.$logs.splice(0, 0, { params: param.replaceAll("&", " "), data: response.data });
            console.log(response.data);
        }, function (error) {
            console.log(error);
        });
    };

    function validateSize() {
        $scope.$invalidSize = false;
        $scope.$unknownSize = false;
        $scope.$sizeValidationMessage = "";

        if (!$scope.squareMts) return;

        var param = $.param(getParams());
        var url = "api/property/size/predict?" + param;
        $http.get(url).then(function (response) {
            processSizePrediction(response.data);
            $scope.$logs.splice(0, 0, { params: param.replaceAll("&", " "), data: response.data });
        }, function (error) {
            console.log(error);
        });
    };

    var getParams = function () {
        var result = {};
        for (var p in $scope) {
            if (p.substr(0, 1) !== "$" && !$.isFunction($scope[p]) && !$.isArray($scope[p])) {
                result[p] = $scope[p];
            }
        }
        return result;
    };

    var processPricePrediction = function (prediction) {
        $scope.$invalidPrice = false;
        $scope.$priceValidationMessage = "";
        $scope.$priceUnknown = false;
        if (prediction.Value <= 0) {
            $scope.priceValidationMessage = "Imposible predecir valor!";
            $scope.priceUnknown = true;
            return;
        }
        var variaton = prediction.StdDev;
        if (Math.abs($scope.price - prediction.Value) > variaton) {
            $scope.$invalidPrice = true;
            $scope.$priceValidationMessage = getAcceptableRangeString(prediction);
        }
    };

    var processSizePrediction = function (prediction) {
        $scope.$invalidSize = false;
        $scope.$sizeValidationMessage = "";
        $scope.$unknownSize = false;
        if (prediction.Value <= 0) {
            $scope.$sizeValidationMessage = "Imposible predecir valor!";
            $scope.$unknownSize = true;
            return;
        }
        var variaton = prediction.StdDev;
        if (Math.abs($scope.squareMts - prediction.Value) > variaton) {
            $scope.$invalidSize = true;
            $scope.$sizeValidationMessage = getAcceptableRangeString(prediction);
        }
    };

    var getAcceptableRangeString = function (prediction) {
        var min = prediction.Value - prediction.StdDev;
        var max = prediction.Value + prediction.StdDev;
        return "Rango aceptable de '" + numberWithCommas(Math.ceil(min)) + "' a '" + numberWithCommas(Math.floor(max)) + "'";
    }

    var numberWithCommas = function (x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    };

    var delayTimer;
    function delayExecution(fn) {
        clearTimeout(delayTimer);
        delayTimer = setTimeout(fn, 1000);
    }
});
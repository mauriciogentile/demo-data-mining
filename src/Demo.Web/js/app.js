var app = angular.module("Zupla", ["ng"]);

app.controller("newPropertyCtrl", function ($scope, $http) {
    var acceptableVariation = 0.1;

    $scope.bedrooms = 2;
    $scope.bathrooms = 1;
    $scope.operationType = "Sale";
    $scope.propertyType = "Apartment";
    $scope.distanceToStation = "";
    $scope.squareMts = "";
    $scope.age = "";
    $scope.zoneReputation = "Good";
    $scope.hasParking = false;
    $scope.withFurniture = false;
    $scope.hasBackyard = false;
    $scope.price = "";

    $scope.invalidPrice = false;
    $scope.priceUnknown = false;
    $scope.priceValidationMessage = "";

    $scope.invalidSize = false;
    $scope.unknownSize = false;
    $scope.sizeValidationMessage = "";

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

    $scope.$watch("distanceToStation", function() {
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
        $scope.invalidPrice = false;
        $scope.priceUnknown = false;
        $scope.priceValidationMessage = "";

        if (!$scope.price) return;

        var param = $.param(getParams());
        $http.get("api/property/price/predict?" + param).then(function (response) {
            processPricePrediction(response.data);
            console.log(data);
        }, function (error) {
            console.log(error);
        });
    };

    function validateSize() {

        $scope.invalidSize = false;
        $scope.unknownSize = false;
        $scope.sieValidationMessage = "";

        if (!$scope.squareMts) return;

        var param = $.param(getParams());
        $http.get("api/property/size/predict?" + param).then(function (response) {
            processSizePrediction(response.data);
            console.log(data);
        }, function (error) {
            console.log(error);
        });
    };

    var getParams = function () {
        var result = {};
        for (var p in $scope) {
            if (p.substr(0, 1) !== "$" && !$.isFunction($scope[p])) {
                result[p] = $scope[p];
            }
        }
        return result;
    };

    var processPricePrediction = function (prediction) {
        $scope.invalidPrice = false;
        $scope.priceValidationMessage = "";
        $scope.priceUnknown = false;
        if (prediction == 0) {
            $scope.priceValidationMessage = "Imposible predecir valor!";
            $scope.priceUnknown = true;
            return;
        }
        var variaton = prediction * acceptableVariation;
        if (Math.abs($scope.price - prediction) > variaton) {
            $scope.invalidPrice = true;
            $scope.priceValidationMessage = getAcceptableRangeString(prediction);
        }
    };

    var processSizePrediction = function (prediction) {
        $scope.invalidSize = false;
        $scope.sizeValidationMessage = "";
        $scope.unknownSize = false;
        if (prediction == 0) {
            $scope.sizeValidationMessage = "Imposible predecir valor!";
            $scope.unknownSize = true;
            return;
        }
        var variaton = prediction * acceptableVariation;
        if (Math.abs($scope.squareMts - prediction) > variaton) {
            $scope.invalidSize = true;
            $scope.sizeValidationMessage = getAcceptableRangeString(prediction);
        }
    };

    var getAcceptableRangeString = function (value) {
        value = parseInt(value);
        var min = value - (value * acceptableVariation);
        var max = value + (value * acceptableVariation);
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
var app = angular.module("Zupla", ["ng"]);

app.controller("newPropertyCtrl", function ($scope, $http) {
    $scope.bedrooms = 1;
    $scope.bathrooms = 1;
    $scope.operationType = "Sale";
    $scope.propertyType = "Apartment";
    $scope.distanceToStation = 100;
    $scope.squareMts = 100;
    $scope.age = 10;
    $scope.zoneReputation = "Good";
    $scope.hasParking = false;
    $scope.withFurniture = false;
    $scope.hasBackyard = false;
    $scope.price = 0;

    $scope.invalidPrice = false;
    $scope.priceUnknown = false;
    $scope.validationMessage = "";

    $scope.$watch("bedrooms", validatePrice);
    $scope.$watch("bathrooms", validatePrice);
    $scope.$watch("squareMts", validatePrice);
    $scope.$watch("operationType", validatePrice);
    $scope.$watch("propertyType", validatePrice);
    $scope.$watch("distanceToStation", validatePrice);
    $scope.$watch("age", validatePrice);
    $scope.$watch("zoneReputation", validatePrice);
    $scope.$watch("hasParking", validatePrice);
    $scope.$watch("price", validatePrice);

    function validatePrice() {
        if (!$scope.price) return;
        $scope.invalidPrice = false;
        $scope.priceUnknown = false;
        $scope.validationMessage = "";
        var param = $.param(getParams());
        $http.get("api/property/validate/price?" + param).then(function () {
            $scope.invalidPrice = false;
            $scope.validationMessage = "";
        }, function (error) {
            if (error.status == 404) {
                $scope.priceUnknown = true;
                $scope.validationMessage = error.responseText || error.data;
            }
            if (error.status == 406) {
                $scope.invalidPrice = true;
                $scope.validationMessage = error.responseText || error.data;
            }
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
});
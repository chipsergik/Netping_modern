var Res = function($){

    function ResourcesClass() {
        this._defaults = {};
        this._resources = {};
    }

    /**
     * Initializes resources object by passed resources object.
     * @param resources The object that contains key:value paris for localized resources
     */
    ResourcesClass.prototype.init = function(resources) {
        this._resources = $.extend(this._resources, this._defaults, resources);
    };

    ResourcesClass.prototype.get = function(resourceKey) {

        if (!this._resources[resourceKey])
        {
            throw Error("The resource with key " + resourceKey + " was not found.");
        }

        return this._resources[resourceKey];
    };

    return new ResourcesClass();

}(jQuery);
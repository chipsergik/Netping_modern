;(function ( $, window, document, undefined ) {

    var pluginName = "exlist",
        defaults = {
            listItemSelector: null,
            top: 5
        };

    // The actual plugin constructor
    function Plugin ( element, options ) {
        this.element = $(element);
        this.settings = $.extend( {}, defaults, options );
        this._defaults = defaults;
        this._name = pluginName;
        this.init();
    }

    Plugin.prototype = {
        init: function () {
            this.collapse();
            var self = this;
            $('[data-expand]', this.element).click(function(){
                self.expand();
                $(this).hide();
            });
        },

        collapse: function() {
            var self = this;
            $(this.settings.listItemSelector, this.element).each(function(index, e){
                if (index >= self.settings.top)
                {
                    $(this).hide();
                }
            });
        },

        expand: function() {
            var self = this;
            $(this.settings.listItemSelector, this.element).each(function(index, e){
                $(this).show();
            });
        }
    };

    // A really lightweight plugin wrapper around the constructor,
    // preventing against multiple instantiations
    $.fn[ pluginName ] = function ( options ) {
        return this.each(function() {
            if ( !$.data( this, "plugin_" + pluginName ) ) {
                $.data( this, "plugin_" + pluginName, new Plugin( this, options ) );
            }
        });
    };

})( jQuery, window, document );


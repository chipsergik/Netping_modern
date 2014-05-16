(function ($) {

    var pluginName = "sshutter",
        defaults = {
            siblingsSelector: "p",
            duration: 500,
            collapsedClass: 'collapsed',
            expandedClass: 'expanded'
        };

    function Plugin ( element, options ) {
        this.element = element;
        this.settings = $.extend( {}, defaults, options );
        this._defaults = defaults;
        this._name = pluginName;
        this.init();
    }

    Plugin.prototype = {
        init: function () {
            var $element = $(this.element);

            // just a little trick to align text from SP (as it contains spaces)
            $element.text($.trim($element.text()));

            var content = $element.nextUntil(':not(' + this.settings.siblingsSelector + ')');
            content.wrapAll("<div />");
            content.parent().hide();
            var self = this;
            $element.addClass('shutter').addClass(self.settings.collapsedClass)
                .click(function(){
                    if ($(this).hasClass(self.settings.collapsedClass))
                    {
                        $(this).removeClass(self.settings.collapsedClass)
                            .addClass(self.settings.expandedClass)
                            .next('div').slideDown(self.settings.duration);
                    }
                    else
                    {
                        $(this)
                            .removeClass(self.settings.expandedClass)
                            .addClass(self.settings.collapsedClass)
                            .next('div').slideUp(self.settings.duration);
                    }
                });
        }
    };

    $.fn[ pluginName ] = function ( options ) {
        return this.each(function() {
            if ( !$.data( this, "plugin_" + pluginName ) ) {
                $.data( this, "plugin_" + pluginName, new Plugin( this, options ) );
            }
        });
    };

})(jQuery)


videre.registerNamespace('$clientnamespace$');

$clientnamespace$.$clientclassname$ = videre.widgets.base.extend(
{
    //get_data: function() { return this._data; },
    //set_data: function(v) { this._data = v; },

    //constructor
    init: function()
    {
        this._base();  //call base method

        //member variables
        //this._data = null;
        //this._dialog = null;

        //delegates that we will use more than once should be stored in member variable to minimize memory useage/leaks
        this._delegates = {
            //onDataReturn: videre.createDelegate(this, this._onDataReturn)
        };
    },

    //note:  _onload is only event handler we order near the top
    _onLoad: function(src, args)
    {
        this._base(); //call base method

        //hook up our controls onload
        //this._dialog = this.getControl('Dialog').modal('hide');

        this.bind();
    },

    //public methods
    //refresh: function()
    //{
    //    this.ajax('~/$servernamespace$/ToDo/GetTasks', {}, this._delegates.onDataReturn);
    //},

    bind: function()
    {
        this.reset();
        //videre.dataTables.clear(this.getControl('ItemTable'));
        //this.getControl('ItemList').html(this.getControl('ItemListTemplate').render(this._data));
        //this.getControl('ItemList').find('.btn').click(this._delegates.onActionClicked);
        //videre.dataTables.bind(this.getControl('ItemTable'), { aoColumns: [{ bSortable: false }] });
    },

    reset: function()
    {
        this.clearMsgs();
        //this.clearMsgs(this._dialog);
    },

    //edit: function()
    //{
    //    this.reset();
    //    videre.UI.showModal(this._dialog);
    //    this.bindData(this._selectedItem, this._dialog);
    //},

    //save: function()
    //{
    //    if (this.validControls(this._dialog, this._dialog))
    //    {
    //        var item = this.persistData(this._selectedItem, true, this._dialog);
    //        this.ajax('~/$servernamespace$/ToDo/SaveTask', { task: item }, this._delegates.onDataSaveReturn, null, this._dialog);
    //    }
    //},

    //deleteItem: function(id)
    //{
    //    //if (confirm('Are you sure you wish to remove this entry?')) //todo: localize?
    //    var self = this;
    //    videre.UI.confirm('Delete Entry', 'Are you sure you wish to remove this entry?', function ()
    //    {
    //        self.ajax('~/$servernamespace$/ToDo/DeleteTask', { id: id }, self._delegates.onDataSaveReturn);
    //    });
    //},

    //private methods

    //event handlers (private)
    _onDataReturn: function(result, ctx)
    {
        if (!result.HasError)
        {
            this._data = result.Data;
            this.bind();
        }
    }

    //_onActionClicked: function(e)
    //{
    //    var ctl = $(e.target).closest('[data-action]');
    //    this._handleAction(ctl.data('action'), ctl.data('id'));
    //}

});


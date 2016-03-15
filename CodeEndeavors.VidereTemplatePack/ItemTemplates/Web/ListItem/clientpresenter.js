/// <reference path="..\..\..\$videredir$\scripts\videre.extensions.js" />
/// <reference path="..\..\..\$videredir$\scripts\videre.js" />

videre.registerNamespace('$clientnamespace$');
$clientnamespace$.$clientclassname$ = videre.widgets.base.extend(
{
    //properties - these can be easily passed from server - i.e. Html.RegisterControlPresenter("$clientnamespace$.todo", Model, new { data = tasks } )
    get_data: function() { return this._data; },
    set_data: function(v)
    {
        this._data = v;
        this._dataDict = v.toDictionary(function(d) { return d.Id; });  //use extension method (videre.extensions.js) to translate the data list to a dictionary for quick lookups
    },

    //constructor
    init: function()
    {
        this._base();  //call base method

        //member variables
        this._data = null;
        this._dataDict = null;
        this._selectedItem = null;

        this._dialog = null;

        this._delegates = {     //delegates that we will use more than once should be stored in member variable to minimize memory useage/leaks
            onDataReturn: videre.createDelegate(this, this._onDataReturn)
        };
    },

    _onLoad: function(src, args)    //note:  _onload is only event handler we order near the top
    {
        this._base(); //call base method

        //hook up our controls onload
        this.getControl('btnNew').click(videre.createDelegate(this, this._onNewClicked));
        this.getControl('ItemList').click(videre.createDelegate(this, this._onActionClicked));

        //if (this._data != null) 
        //    this.bind();
        //else
        //    this.refresh();
    },

    //public methods
    //refresh: function()
    //{
    //    //this.ajax('~/$clientnamespace$/$safeitemname$/Get', {}, this._delegates.onDataReturn);  /
    //},

    //bind: function()
    //{
    //    videre.dataTables.clear(this.getControl('ItemTable'));  //must clear datatables in order to build a new one
    //    this.getControl('ItemList').html(this.getControl('ItemListTemplate').render(this._data));       //call to jsRender template
    //    videre.dataTables.bind(this.getControl('ItemTable'), { order: [1, 'asc'], columnDefs: [{ orderable: false, targets: 0 }] });    //convert table to datatable setting sort defaults
    //},

    //reset: function()
    //{
    //    this.clearMsgs();   //clear up any existing error messages
    //    this.clearMsgs(this._dialog);
    //},

    //newItem: function()
    //{
    //    this._selectedItem = { Name: '', Description: '', CompleteDate: null };
    //    this.edit();
    //},

    //edit: function()
    //{
    //},

    //deleteItem: function(id)
    //{
    //    var self = this;    //since we are using a closure, in order for onDataSaveReturn to be accessible, we need to store a reference to this presenter
    //    videre.UI.confirm('Delete Entry', 'Are you sure you wish to remove this entry?', function()    //use built in dialog logic to show confirm dialog
    //    {
    //        //self.ajax('~/$clientnamespace$/$safeitemname$/Delete', { id: id }, self._delegates.onDataSaveReturn);
    //    });
    //},

    ////private methods
    //_handleAction: function(action, id)
    //{
    //    this._selectedItem = this._dataDict[id];
    //    if (this._selectedItem != null)
    //    {
    //        if (action == 'edit')
    //            this.edit();
    //        else if (action == 'delete')
    //            this.deleteItem(id);
    //    }
    //},

    ////event handlers (private)
    //_onDataReturn: function(result, ctx)
    //{
    //    if (!result.HasError)   //verify result has no errors (if there was an error framework will show it in UI automatically)
    //    {
    //        this.set_data(result.Data);
    //        this.bind();
    //    }
    //},

    //_onActionClicked: function(e)
    //{
    //    var ctl = $(e.target).closest('[data-action]');
    //    if (ctl.data('id') != null) //since we bound our click to tbody we need to ensure that we clicked on something that had an action/id
    //        this._handleAction(ctl.data('action'), ctl.data('id'));
    //},
    
    //_onNewClicked: function(e)
    //{
    //    this.newItem();
    //}


});

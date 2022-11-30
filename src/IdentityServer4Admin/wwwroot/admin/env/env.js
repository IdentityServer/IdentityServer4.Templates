/**
 * Created by Sam on 07/12/2016.
 */

var ENV = {<% _.forEach(env, function(v, k) { %>
    <%= k %>: <%= _.isString(v) ? "\'"+v+"\'" : v %>,<% }) %>
};
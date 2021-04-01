# -*- coding: utf-8 -*-
"""
@author: fanouriaath
"""
import os
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns

from random import sample
from sklearn.linear_model import LogisticRegression
from sklearn import tree
from sklearn.ensemble import RandomForestClassifier
from sklearn.svm import SVC
from sklearn.neighbors import KNeighborsClassifier
from sklearn.naive_bayes import GaussianNB
from sklearn.cross_validation import cross_val_score
from sklearn import metrics
from IPython.display import Image
from pydotplus import graph_from_dot_data

#read measurements
measurements = pd.read_csv('measurements.csv') # Measurements' table from fwaves.db
measurements.head()
print(measurements.groupby('DIAGNOSIS').size()) 
measurementsid = pd.read_csv('measurementsid.csv') # Measurements' ID table from fwaves.db
measurementsid.head()



#cmap = cm.get_cmap('gnuplot')
#scatter = pd.scatter_matrix(X, c = y, marker = 'o', s=40, hist_kwds={'bins':15}, figsize=(9,9), cmap = cmap)
#plt.suptitle('Scatter-matrix for each input variable')
#plt.savefig('measurements_scatter_matrix')
#%% 
"""
Create additional Features
"""

AMKA = measurements['AMKA']
computations = measurements.set_index("AMKA", drop = False)
FCVmean = []
FCVvar = []
FCVmin = []
FCVmax = []

# FCV percentages into different size bins:  <48, 48-56, 60-64,64-70,70-74,74-86,>86
FCV1 = []
FCV2 = []
FCV3 = []
FCV4 = []
FCV5 = []
FCV6 = []
FCV7 = []
fcv1 = 0
fcv2 = 0
fcv3 = 0
fcv4 = 0 
fcv5 = 0
fcv6 = 0
fcv7 = 0 
lim1 = 48
lim2 = 56
lim3 = 64
lim4 = 70
lim5 = 74
lim6 = 86

AMPmean = []
for i in range( 1, len(measurements)-1):           
    if AMKA.iloc[i] != AMKA.loc[i+1]:
        df = computations.loc[computations['AMKA'] == AMKA.iloc[i], 'FCV'] # Series containing FCV column for a certain measurement
        df2 = computations.loc[computations['AMKA'] == AMKA.iloc[i], 'AMPLITUDE'] # Series containing AMPLITUDE column for a certain measurement
        for row in  df.iteritems():
            if (row[1] <= lim1):
                fcv1 = fcv1 + 1
            elif (row[1] <= lim2):
                fcv2 = fcv2 + 1
            elif (row[1] >= lim2 and row[1] <= lim3):
                fcv3 = fcv3 + 1
            elif (row[1] <= lim4):
                fcv4 = fcv4 + 1
            elif (row[1] <= lim5):
                fcv5 = fcv5 + 1    
            elif (row[1] <= lim6):
                fcv6 = fcv6 + 1
            else:
                fcv7 = fcv7 + 1                
        counter = df.shape      
        FCV1.append(fcv1 / counter[0])
        FCV2.append(fcv2 / counter[0])
        FCV3.append(fcv3 / counter[0])
        FCV4.append(fcv4 / counter[0])
        FCV5.append(fcv5 / counter[0])
        FCV6.append(fcv6 / counter[0])
        FCV7.append(fcv7 / counter[0])
        fcv1 = 0
        fcv2 = 0
        fcv3 = 0
        fcv4 = 0 
        fcv5 = 0
        fcv6 = 0
        fcv7 = 0 
        FCVmean.append(df.mean())
        FCVvar.append(df.var())
        FCVmin.append(df.min())
        FCVmax.append(df.max())
        AMPmean.append(df2.mean())

df = computations.loc[computations['AMKA'] == AMKA.iloc[len(measurements)-1], 'FCV']
counter = df.shape      
FCV1.append(fcv1 / counter[0])
FCV2.append(fcv2 / counter[0])
FCV3.append(fcv3 / counter[0])
FCV4.append(fcv4 / counter[0])
FCV5.append(fcv5 / counter[0])
FCV6.append(fcv6 / counter[0])
FCV7.append(fcv7 / counter[0])
FCVmean.append(df.mean())
FCVvar.append(df.var())
FCVmin.append(df.min())
FCVmax.append(df.max())
AMPmean.append(df2.mean()) 

measurementsid['FCVmean'] = pd.Series(FCVmean)
measurementsid['FCVvar'] = pd.Series(FCVvar)
measurementsid['FCVmin'] = pd.Series(FCVmin)
measurementsid['FCVmax'] = pd.Series(FCVmax)
measurementsid['AMPmean'] = pd.Series(AMPmean)
measurementsid['FCV1'] = pd.Series(FCV1)
measurementsid['FCV2'] = pd.Series(FCV2)
measurementsid['FCV3'] = pd.Series(FCV3)
measurementsid['FCV4'] = pd.Series(FCV4)
measurementsid['FCV5'] = pd.Series(FCV5)
measurementsid['FCV6'] = pd.Series(FCV6)
measurementsid['FCV7'] = pd.Series(FCV7)

#%% 
"""
Choose Features
"""
names = [ 'Mlat' , 'Marea'     , 'Mamp'   , 
         'FPer'  , 'FRepPer'   , 'RNs' , 
         'RN5Rep', 'FrepsTotal', 'FCVmean', 
         'Sex'   , 'AMPmean', 'FCV1', 'FCV2', 'FCV3', 'FCV4', 'FCV5', 'FCV6', 'FCV7']

feature_names = names
X = measurementsid[feature_names]
Y = measurementsid['Diagnosis']

#%%        
"""
Visualization
"""
#measurementsid.drop('Diagnosis' ,axis=1).hist(bins=30, figsize=(9,9))
#pl.suptitle("Histogram for each numeric input variable")
#plt.savefig('measurementsid_hist')
#plt.show()
#from pandas.tools.plotting import scatter_matrix
#from matplotlib import cm
#
#cmap = cm.get_cmap('gnuplot')
#scatter = pd.scatter_matrix(X, c = Y, marker = 'o', s=40,
#                            hist_kwds={'bins':15}, figsize=(9,9), cmap = cmap)
#plt.suptitle('Scatter-matrix for each input variable')
#plt.savefig('measurementsid_scatter_matrix')

#%%        
"""
Create Training and Test Sets and Apply Scaling
"""
seed = 0
from sklearn.model_selection import train_test_split
x_train, x_test, y_train, y_test = train_test_split(X, Y, random_state=seed)
from sklearn.preprocessing import MinMaxScaler
scaler = MinMaxScaler()
# Scale the Training and Test Sets
x_train = scaler.fit_transform(x_train)
x_test = scaler.transform(x_test) 

#%%
"""
Build Models
"""

#Logistic Regression 
from sklearn.linear_model import LogisticRegression
logreg = LogisticRegression()
logreg.fit(x_train, y_train)
print('Accuracy of Logistic regression classifier on training set: {:.2f}'
     .format(logreg.score(x_train, y_train)))
print('Accuracy of Logistic regression classifier on test set: {:.2f}'
     .format(logreg.score(x_test, y_test)))

#Decision Tree 
from sklearn.tree import DecisionTreeClassifier
clf = DecisionTreeClassifier().fit(x_train, y_train)
print('Accuracy of Decision Tree classifier on training set: {:.2f}'
     .format(clf.score(x_train, y_train)))
print('Accuracy of Decision Tree classifier on test set: {:.2f}'
     .format(clf.score(x_test, y_test)))
print(clf.feature_importances_)
#K-Nearest Neighbors
from sklearn.neighbors import KNeighborsClassifier
knn = KNeighborsClassifier()
knn.fit(x_train, y_train)
print('Accuracy of K-NN classifier on training set: {:.2f}' 
      .format(knn.score(x_train, y_train)))
print('Accuracy of K-NN classifier on test set: {:.2f}' 
      .format(knn.score(x_test, y_test)))

#Linear Discriminant Analysis
from sklearn.discriminant_analysis import LinearDiscriminantAnalysis
lda = LinearDiscriminantAnalysis()
lda.fit(x_train, y_train)
print('Accuracy of LDA classifier on training set: {:.2f}'
     .format(lda.score(x_train, y_train)))
print('Accuracy of LDA classifier on test set: {:.2f}'
     .format(lda.score(x_test, y_test)))

#Gaussian Naive Bayes
from sklearn.naive_bayes import GaussianNB
gnb = GaussianNB()
gnb.fit(x_train, y_train)
print('Accuracy of GNB classifier on training set: {:.2f}'
     .format(gnb.score(x_train, y_train)))
print('Accuracy of GNB classifier on test set: {:.2f}'
     .format(gnb.score(x_test, y_test)))

#Support Vector Machine
from sklearn.svm import SVC
svm = SVC()
svm.fit(x_train, y_train)
print('Accuracy of SVM classifier on training set: {:.2f}'
     .format(svm.score(x_train, y_train)))
print('Accuracy of SVM classifier on test set: {:.2f}'
     .format(svm.score(x_test, y_test)))

#%%
"""
Models Evaluation
"""
from sklearn import model_selection

seed = 6
kfold = model_selection.KFold(n_splits=10, random_state=seed)

#Accuracy
scoring = 'accuracy'
results = model_selection.cross_val_score(gnb, X, Y, cv=kfold, scoring=scoring)
print('Cross Validation - Accuracy of GNB: {:.3f} ({:.3f})' 
      .format(results.mean(), results.std()))

#Log_loss
#scoring = 'neg_log_loss'
#results = model_selection.cross_val_score(gnb, X, Y, cv=kfold, scoring=scoring)
#print('Logloss of GNB: {:.2f} ({:.2f})' 
#      .format(results.mean(), results.std()))

#scoring = 'roc_auc'
#results = model_selection.cross_val_score(gnb, X, Y, cv=kfold, scoring=scoring)
#print('Roc Auc: {:.3f} ({:.3f})' 
#      .format(results.mean(), results.std()))
#"""
#Evaluation Matrix
#"""
#from sklearn.feature_selection import SelectKBest, chi2
#X_new = SelectKBest(chi2, k='all').fit_transform(X, Y)
#
#from tabulate import tabulate
#from math import sqrt
#
#
#def mysqrt(a):
#    for x in range(1, int(1 / 2 * a)):
#        while True:
#            y = (x + a / x) / 2
#            ifjl y == x:
#                break
#            x = y
#    return x
#
#
#results = [(x, mysqrt(x), sqrt(x)) for x in range(10, 20)]
#print(tabulate(results, headers=["num", "mysqrt", "sqrt"]))

#%% EVALUATE
# Variable importance / selection
rf = RandomForestClassifier()
rf.fit(x_train, y_train)
print ("Features sorted by their score:")
#print(sorted(zip(map(lambda x: round(x, 4), rf.feature_importances_), x_train), reverse=True))

# Assign feature importance and sort
importances = rf.feature_importances_
std = np.std([rf.feature_importances_ for tree in rf.estimators_], axis=0)
indices = np.argsort(importances)[::-1]

# Plot variable importance
plt.figure()
plt.title("Feature importance")
plt.bar(range(x_train.shape[1]), importances[indices], color="r", yerr=std[indices], align="center")

## Create variable lists and drop
#all_vars = x_train.columns.tolist()
#top_5_vars = ['satisfaction_level', 'number_project', 'time_spend_company',
#              'average_montly_hours', 'last_evaluation']
#bottom_vars = [cols for cols in all_vars if cols not in top_5_vars]
#
## Drop less important variables leaving the top_5
#x_train    = x_train.drop(bottom_vars, axis=1)
#x_test     = x_test.drop(bottom_vars, axis=1)
#x_validate = x_validate.drop(bottom_vars, axis=1)

# LOGISTIC REGRESSION
# Instantiate, fit and obtain accuracy score
logit_model = LogisticRegression()
logit_model = logit_model.fit(x_train, y_train)
logit_model.score(x_train, y_train)

# Examine the coefficients
pd.DataFrame(zip(x_train.columns, np.transpose(logit_model.coef_)))

# Predictions on the test dataset
predicted = pd.DataFrame(logit_model.predict(x_test))
print(predicted.head(n=15))

# Probabilities on the test dataset
probs = pd.DataFrame(logit_model.predict_proba(x_test))
print(probs.head(n=15))

# Store metrics
logit_accuracy = metrics.accuracy_score(y_test, predicted)
logit_roc_auc = metrics.roc_auc_score(y_test, probs[1])
logit_confus_matrix = metrics.confusion_matrix(y_test, predicted)
logit_classification_report = metrics.classification_report(y_test, predicted)
logit_precision = metrics.precision_score(y_test, predicted, pos_label=1)
logit_recall = metrics.recall_score(y_test, predicted, pos_label=1)
logit_f1 = metrics.f1_score(y_test, predicted, pos_label=1)

# Evaluate the model using 10-fold cross-validation
logit_cv_scores = cross_val_score(LogisticRegression(), x_test, y_test, scoring='precision', cv=10)
logit_cv_mean = np.mean(logit_cv_scores)

# DECISION TREE (pruned to depth of 3)
# TODO optimise depth

# Instantiate with a max depth of 3
tree_model = tree.DecisionTreeClassifier(max_depth=3)
# Fit a decision tree
tree_model = tree_model.fit(x_train, y_train)
# Training accuracy
tree_model.score(x_train, y_train)

# Predictions/probs on the test dataset
predicted = pd.DataFrame(tree_model.predict(x_test))
probs = pd.DataFrame(tree_model.predict_proba(x_test))

# Store metrics
tree_accuracy = metrics.accuracy_score(y_test, predicted)
tree_roc_auc = metrics.roc_auc_score(y_test, probs[1])
tree_confus_matrix = metrics.confusion_matrix(y_test, predicted)
tree_classification_report = metrics.classification_report(y_test, predicted)
tree_precision = metrics.precision_score(y_test, predicted, pos_label=1)
tree_recall = metrics.recall_score(y_test, predicted, pos_label=1)
tree_f1 = metrics.f1_score(y_test, predicted, pos_label=1)

# Evaluate the model using 10-fold cross-validation
tree_cv_scores = cross_val_score(tree.DecisionTreeClassifier(max_depth=3),
                                x_test, y_test, scoring='precision', cv=10)
tree_cv_mean = np.mean(tree_cv_scores)

# Output decision plot
dot_data = tree.export_graphviz(tree_model, out_file=None,
                     feature_names=x_test.columns.tolist(),
                     class_names=['remain', 'left'],
                     filled=True, rounded=True,
                     special_characters=True)
graph = graph_from_dot_data(dot_data)
graph.write_png("images/decision_tree.png")

# RANDOM FOREST
# Instantiate
rf = RandomForestClassifier()
# Fit
rf_model = rf.fit(x_train, y_train)
# Training accuracy
rf_model.score(x_train, y_train)

# Predictions/probs on the test dataset
predicted = pd.DataFrame(rf_model.predict(x_test))
probs = pd.DataFrame(rf_model.predict_proba(x_test))

# Store metrics
rf_accuracy = metrics.accuracy_score(y_test, predicted)
rf_roc_auc = metrics.roc_auc_score(y_test, probs[1])
rf_confus_matrix = metrics.confusion_matrix(y_test, predicted)
rf_classification_report = metrics.classification_report(y_test, predicted)
rf_precision = metrics.precision_score(y_test, predicted, pos_label=1)
rf_recall = metrics.recall_score(y_test, predicted, pos_label=1)
rf_f1 = metrics.f1_score(y_test, predicted, pos_label=1)

# Evaluate the model using 10-fold cross-validation
rf_cv_scores = cross_val_score(RandomForestClassifier(), x_test, y_test, scoring='precision', cv=10)
rf_cv_mean = np.mean(rf_cv_scores)

# SUPPORT VECTOR MACHINE
# Instantiate
svm_model = SVC(probability=True)
# Fit
svm_model = svm_model.fit(x_train, y_train)
# Accuracy
svm_model.score(x_train, y_train)

# Predictions/probs on the test dataset
predicted = pd.DataFrame(svm_model.predict(x_test))
probs = pd.DataFrame(svm_model.predict_proba(x_test))

# Store metrics
svm_accuracy = metrics.accuracy_score(y_test, predicted)
svm_roc_auc = metrics.roc_auc_score(y_test, probs[1])
svm_confus_matrix = metrics.confusion_matrix(y_test, predicted)
svm_classification_report = metrics.classification_report(y_test, predicted)
svm_precision = metrics.precision_score(y_test, predicted, pos_label=1)
svm_recall = metrics.recall_score(y_test, predicted, pos_label=1)
svm_f1 = metrics.f1_score(y_test, predicted, pos_label=1)

# Evaluate the model using 10-fold cross-validation
svm_cv_scores = cross_val_score(SVC(probability=True), x_test, y_test, scoring='precision', cv=10)
svm_cv_mean = np.mean(svm_cv_scores)

# KNN
# Instantiate learning model (k = 3)
knn_model = KNeighborsClassifier(n_neighbors=3)
# Fit the model
knn_model.fit(x_train, y_train)
# Accuracy
knn_model.score(x_train, y_train)

# Predictions/probs on the test dataset
predicted = pd.DataFrame(knn_model.predict(x_test))
probs = pd.DataFrame(knn_model.predict_proba(x_test))

# Store metrics
knn_accuracy = metrics.accuracy_score(y_test, predicted)
knn_roc_auc = metrics.roc_auc_score(y_test, probs[1])
knn_confus_matrix = metrics.confusion_matrix(y_test, predicted)
knn_classification_report = metrics.classification_report(y_test, predicted)
knn_precision = metrics.precision_score(y_test, predicted, pos_label=1)
knn_recall = metrics.recall_score(y_test, predicted, pos_label=1)
knn_f1 = metrics.f1_score(y_test, predicted, pos_label=1)

# Evaluate the model using 10-fold cross-validation
knn_cv_scores = cross_val_score(KNeighborsClassifier(n_neighbors=3), x_test, y_test, scoring='precision', cv=10)
knn_cv_mean = np.mean(knn_cv_scores)

# TWO CLASS BAYES
# Instantiate
bayes_model = GaussianNB()
# Fit the model
bayes_model.fit(x_train, y_train)
# Accuracy
bayes_model.score(x_train, y_train)

# Predictions/probs on the test dataset
predicted = pd.DataFrame(bayes_model.predict(x_test))
probs = pd.DataFrame(bayes_model.predict_proba(x_test))

# Store metrics
bayes_accuracy = metrics.accuracy_score(y_test, predicted)
bayes_roc_auc = metrics.roc_auc_score(y_test, probs[1])
bayes_confus_matrix = metrics.confusion_matrix(y_test, predicted)
bayes_classification_report = metrics.classification_report(y_test, predicted)
bayes_precision = metrics.precision_score(y_test, predicted, pos_label=1)
bayes_recall = metrics.recall_score(y_test, predicted, pos_label=1)
bayes_f1 = metrics.f1_score(y_test, predicted, pos_label=1)

# Evaluate the model using 10-fold cross-validation
bayes_cv_scores = cross_val_score(KNeighborsClassifier(n_neighbors=3), x_test, y_test, scoring='precision', cv=10)
bayes_cv_mean = np.mean(bayes_cv_scores)

# Model comparison
models = pd.DataFrame({
    'Model'    : ['Logistic Regression', 'Decision Tree', 'Random Forest', 'SVM', 'kNN', 'Bayes'],
    'Accuracy' : [logit_accuracy, tree_accuracy, rf_accuracy, svm_accuracy, knn_accuracy, bayes_accuracy],
    'Precision': [logit_precision, tree_precision, rf_precision, svm_precision, knn_precision, bayes_precision],
    'recall'   : [logit_recall, tree_recall, rf_recall, svm_recall, knn_recall, bayes_recall],
    'F1'       : [logit_f1, tree_f1, rf_f1, svm_f1, knn_f1, bayes_f1],
    'cv_precision' : [logit_cv_mean, tree_cv_mean, rf_cv_mean, svm_cv_mean, knn_cv_mean, bayes_cv_mean]
    })
models.sort_values(by='Precision', ascending=False)

# Create x and y from all data
y = df['left']
x = df.drop(['left'], axis=1)

# Re-train model on all data
rf_model = rf.fit(x, y)

# Save model
import cPickle
with open('churn_classifier.pkl', 'wb') as fid:
    cPickle.dump(rf_model, fid)
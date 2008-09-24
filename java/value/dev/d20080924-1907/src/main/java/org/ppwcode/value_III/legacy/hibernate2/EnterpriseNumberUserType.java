/*<license>
Copyright 2004 - $Date$ by PeopleWare n.v..

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</license>*/

package org.ppwcode.value_III.legacy.hibernate2;


import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Types;

import net.sf.hibernate.HibernateException;
import net.sf.hibernate.UserType;

import org.ppwcode.value_III.legacy.EnterpriseNumber;
import org.ppwcode.vernacular.value_III.PropertyException;


/**
 * Hibernate user type for EnterpriseNumber.
 * Instances of {@link EnterpriseNumber} are mapped to 1 column in the database.
 *
 * @author    Jan Dockx
 * @author    PeopleWare n.v.
 */
public class EnterpriseNumberUserType implements UserType {

  /*<section name="Meta Information">*/
  //  ------------------------------------------------------------------

  /** {@value} */
  public static final String CVS_REVISION = "$Revision$"; //$NON-NLS-1$
  /** {@value} */
  public static final String CVS_DATE = "$Date$"; //$NON-NLS-1$
  /** {@value} */
  public static final String CVS_STATE = "$State$"; //$NON-NLS-1$
  /** {@value} */
  public static final String CVS_TAG = "$Name$"; //$NON-NLS-1$

  /*</section>*/

  /**
   * Create a new {@link EnterpriseNumberUserType}.
   */
  public EnterpriseNumberUserType() {
    // NOP
  }

  private static final int[] SQL_TYPES = {Types.VARCHAR};

  /**
   * @return {Types.VARCHAR};
   */
  public final int[] sqlTypes() {
    return SQL_TYPES;
  }

  /**
   * @return NationalNumber.class;
   */
  public Class returnedClass() {
    return EnterpriseNumber.class;
  }

  /**
   * @return (x == null) ? (y == null) : x.equals(y);
   */
  public final boolean equals(final Object x, final Object y) {
    return (x == null)
               ? (y == null)
               : x.equals(y);
  }

  /**
   * Since this is an immutable class, we needn't make a copy.
   * @return value;
   */
  public Object deepCopy(final Object value) {
    return value;
  }

  /**
   * @return false;
   */
  public final boolean isMutable() {
    return false;
  }

  /**
   * Retrieve an instance of the mapped class from a JDBC resultset.
   * Implementors should handle possibility of null values.
   *
   * @result  resultSet.wasNull()
   *            ==> result == null;
   * @result  !resultSet.wasNull()
   *            ==> a Enterprise number is created from the given string
   * @throws  HibernateException
   *          The EnterpriseNumber cannot be created from the given string.
   *          (new EnterpriseNumber(resultSet.getString(names[0])) throws a
   *           PropertyException)
   * @throws  SQLException
   *          resultSet.getString(names[0]);
   * @throws  SQLException
   *          resultSet.wasNull();
   */
  public final Object nullSafeGet(final ResultSet resultSet,
                                  final String[] names,
                                  final Object owner)
          throws HibernateException, SQLException {
    EnterpriseNumber result = null;
    String dbValue = null;
    try {
      dbValue = resultSet.getString(names[0]);
      if (! resultSet.wasNull()) {
        result = new EnterpriseNumber(dbValue);
      }
      // else, result stays null (NULL in DB)
    }
    catch (PropertyException pExc) {
      throw new HibernateException("could not convert string \""
                                   + dbValue + "\" from DB to EnterpriseNumber", pExc);
    }
    return result;
  }

  /**
   * Write an instance of the mapped class to a prepared statement.
   * Implementors should handle possibility of null values.
   * A multi-column type should be written to parameters starting from index.
   *
   * @post    value == null
   *            ==> the parameter at the given index is set to null
   * @post    value != null
   *            ==> the parameter at the given index is set to the concatenation
   *                of left, middle and right number of the given
   *                Enterprise number
   *
   * @throws  HibernateException
   *          (value != null)
              && !returnedClass().getName().equals(value.getClass().getName())
   * @throws  SQLException
   *          value == null
   *          && statement.setNull(index, Types.VARCHAR) throws a SQLException;
   * @throws  SQLException
   *          value != null
   *          && statement.setString(
   *                 index,
   *                 ((EnterpriseNumber) value).getLeftNumber()
   *                 + (EnterpriseNumber) value.getMiddleNumber()
   *                 + (EnterpriseNumber) value.getRightNumber());
   */
  public final void nullSafeSet(final PreparedStatement statement,
                                final Object value,
                                final int index)
      throws HibernateException, SQLException {
    // make sure the received value is of the right type
    if ((value != null) &&
        (! returnedClass().getName().equals(value.getClass().getName()))) {
      throw new HibernateException("\""
                                   + ((value == null)
                                        ? "null"
                                        : value.toString())
                                   + "\" is not a "
                               + returnedClass().getName());
    }
    if (value == null) {
      statement.setNull(index, Types.VARCHAR);
    }
    else {
      EnterpriseNumber nn = (EnterpriseNumber)value;
      statement.setString(index, nn.getLeftNumber() + nn.getMiddleNumber() + nn.getRightNumber());
    }
  }

}
